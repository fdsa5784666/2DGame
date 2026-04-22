using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 武器控制器基类 - 所有武器的抽象基类
/// </summary>
public abstract class WeaponController
{
    protected WeaponInstance weapon;
    protected Transform playerTransform;
    protected LayerMask enemyLayer;
    protected float attackTimer;
    protected float attackCooldown;

    // 状态
    protected bool isInitialized = false;
    protected bool isAttacking = false;

    // 缓存
    protected Transform currentTarget;
    protected List<Transform> cachedTargets = new List<Transform>();

    // 属性
    public WeaponInstance Weapon => weapon;
    public bool IsInitialized => isInitialized;
    public float AttackProgress => attackCooldown > 0 ? attackTimer / attackCooldown : 0;

    /// <summary>
    /// 初始化控制器
    /// </summary>
    public virtual void Initialize(WeaponInstance weaponInstance, Transform player, LayerMask enemyLayers)
    {
        this.weapon = weaponInstance;
        this.playerTransform = player;
        this.enemyLayer = enemyLayers;

        attackCooldown = 1f / weapon.FinalAttackSpeed;
        attackTimer = Random.Range(0f, attackCooldown * 0.5f); // 随机偏移，避免所有武器同时攻击

        isInitialized = true;
        OnInitialized();
    }

    /// <summary>
    /// 初始化后的回调
    /// </summary>
    protected virtual void OnInitialized() { }

    /// <summary>
    /// 每帧更新
    /// </summary>
    public virtual void Update(float deltaTime, Transform nearestEnemy)
    {
        if (!isInitialized) return;
        if (GameManager.Instance?.CurrentState != GameState.Playing) return;

        currentTarget = nearestEnemy;

        attackTimer += deltaTime;

        while (attackTimer >= attackCooldown)
        {
            attackTimer -= attackCooldown;
            TryAttack();
        }
    }

    /// <summary>
    /// 尝试攻击
    /// </summary>
    protected virtual void TryAttack()
    {
        if (!CanAttack()) return;

        Attack();
        OnAttack();
    }

    /// <summary>
    /// 检查是否可以攻击
    /// </summary>
    protected virtual bool CanAttack()
    {
        return playerTransform != null;
    }

    /// <summary>
    /// 执行攻击（子类必须实现）
    /// </summary>
    protected abstract void Attack();

    /// <summary>
    /// 攻击后的回调
    /// </summary>
    protected virtual void OnAttack() { }

    /// <summary>
    /// 强制攻击（用于特殊触发）
    /// </summary>
    public virtual void ForceAttack(Transform target)
    {
        currentTarget = target;
        Attack();
        OnAttack();
    }

    /// <summary>
    /// 武器升级时的回调
    /// </summary>
    public virtual void OnUpgraded(int newLevel)
    {
        attackCooldown = 1f / weapon.FinalAttackSpeed;
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public virtual void Cleanup() { }

    /// <summary>
    /// 播放升级特效
    /// </summary>
    public virtual void PlayLevelUpEffect() { }

    #region 辅助方法
    /// <summary>
    /// 计算最终伤害
    /// </summary>
    protected virtual float CalculateDamage(out bool isCrit)
    {
        float baseDamage = weapon.FinalDamage;

        // 暴击判定
        float critRate = GameManager.Instance?.globalCritRate ?? 0.05f;
        isCrit = Random.value < critRate;

        float damage = baseDamage;
        if (isCrit)
        {
            float critDamage = GameManager.Instance?.globalCritDamage ?? 1.5f;
            damage *= critDamage;
        }

        // 全局增伤
        float globalMultiplier = GameManager.Instance?.globalDamageMultiplier ?? 1f;
        damage *= globalMultiplier;

        // 随机浮动 ±5%
        damage *= Random.Range(0.95f, 1.05f);

        return Mathf.Max(1f, Mathf.Round(damage));
    }

    /// <summary>
    /// 创建子弹信息
    /// </summary>
    protected virtual BulletInfo CreateBulletInfo()
    {
        float damage = CalculateDamage(out bool isCrit);

        var bulletData = weapon.Data.bulletData;
        if (bulletData == null)
        {
            Debug.LogError($"武器 {weapon.Data.weaponName} 没有子弹数据！");
            return new BulletInfo { damage = damage };
        }

        return new BulletInfo
        {
            damage = damage,
            speed = weapon.FinalBulletSpeed,
            size = weapon.FinalBulletSize,
            lifeTime = bulletData.lifeTime,

            pierceCount = weapon.FinalPierceCount,
            homing = bulletData.homing,
            homingStrength = bulletData.homingStrength,
            splitCount = bulletData.splitCount,
            splitAngle = bulletData.splitAngle,
            explosionRadius = bulletData.explosionRadius,
            explosionDamageMultiplier = bulletData.explosionDamageMultiplier,

            bulletColor = bulletData.bulletColor,
            hitEffectPrefab = bulletData.hitEffectPrefab,
            trailEffectPrefab = bulletData.trailEffectPrefab,

            isCritical = isCrit
        };
    }

    /// <summary>
    /// 获取攻击方向
    /// </summary>
    protected virtual Vector2 GetAttackDirection()
    {
        if (currentTarget != null)
        {
            return (currentTarget.position - playerTransform.position).normalized;
        }

        // 默认使用玩家的 facing 方向
        return playerTransform.right;
    }

    /// <summary>
    /// 查找范围内的敌人
    /// </summary>
    protected virtual List<Transform> FindEnemiesInRange(float range)
    {
        cachedTargets.Clear();

        var hits = Physics2D.OverlapCircleAll(playerTransform.position, range, enemyLayer);
        foreach (var hit in hits)
        {
            cachedTargets.Add(hit.transform);
        }

        // 按距离排序
        cachedTargets.Sort((a, b) =>
            Vector2.Distance(playerTransform.position, a.position)
            .CompareTo(Vector2.Distance(playerTransform.position, b.position)));

        return cachedTargets;
    }

    /// <summary>
    /// 查找最近的敌人
    /// </summary>
    protected virtual Transform FindNearestEnemy(float maxRange = 20f)
    {
        var enemies = FindEnemiesInRange(maxRange);
        return enemies.Count > 0 ? enemies[0] : null;
    }
    #endregion
}