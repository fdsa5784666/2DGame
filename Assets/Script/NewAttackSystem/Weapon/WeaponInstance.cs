using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运行时武器实例 - 负责攻击逻辑
/// </summary>
public abstract class WeaponInstance : MonoBehaviour
{
    public WeaponData Data { get; private set; }
    public int StarLevel { get; private set; } = 1;
    public List<EWeaponTag> tags => Data.tags;

    protected Transform attackPoint;
    protected float attackTimer;
    protected float finalDamage;
    protected float finalInterval;

    protected float globalDamageMult = 1f;
    protected float globalAttackSpeedBonus = 0f;
    // 来自升级的额外加成
    //public float damageBonus = 0f;
    //public float attackSpeedBonus = 0f;
    //public float rangeBonus = 0f;
    //public int projectileBonus = 0;
    //public float bulletSpeedBonus = 0f;
    //public float bulletSizeBonus = 0f;
    //public int pierceBonus = 0;

    //public bool CanLevelUp => level < Data.maxLevel;

    // 计算属性
    //public float FinalDamage => data.baseDamage * data.GetDamageMultiplier(level) * (1 + damageBonus);
    //public float FinalAttackSpeed => data.baseAttackSpeed * data.GetAttackSpeedMultiplier(level) * (1 + attackSpeedBonus);
    //public int FinalProjectileCount => Mathf.RoundToInt(data.baseProjectileCount * data.GetProjectileMultiplier(level)) + projectileBonus;
    //public float FinalRange => data.baseRange + rangeBonus;
    //public int FinalPierceCount => (data.bulletData?.pierceCount ?? 1) + pierceBonus;
    //public float FinalBulletSpeed => (data.bulletData?.speed ?? 8f) * (1 + bulletSpeedBonus);
    //public float FinalBulletSize => (data.bulletData?.size ?? 1f) * (1 + bulletSizeBonus);

    public virtual void Initialize(WeaponData data, int starLevel = 1)
    {
        Initialize(data, attackPoint, starLevel);
    }
    public virtual void Initialize(WeaponData data, Transform attackPoint,int starLevel = 1)
    {
        Data = data;
        this.attackPoint = attackPoint != null ? attackPoint : transform;
        StarLevel = starLevel;

        //监听全局属性变化
        if (GameData.Instance != null)
        {
            globalDamageMult = GameData.Instance.GlobalDamageBonus;
            //globalAttackSpeedMult = GameData.Instance.globalAttackSpeedBonus;
        }

        RefreshStats();
    }
    public void LevelUp()
    {
        if(StarLevel < Data.maxLevel)
            StarLevel++;
        RefreshStats();
    }
    public bool IsMaxStar => StarLevel >= Data.maxLevel;
    public bool CanCombineWith(WeaponInstance other)
    {
        return other != null &&
            other.Data == this.Data &&
            other.StarLevel == this.StarLevel &&
            !this.IsMaxStar;
    }   
    public virtual void RefreshStats()
    {
        finalDamage = Data.GetDamage(StarLevel) * globalDamageMult;
        float finalAttackSpeed = Data.GetAttackSpeed(StarLevel) * (1 + globalAttackSpeedBonus);

        finalInterval = 1f / Mathf.Max(0.1f, finalAttackSpeed);
    }
    /// <summary>
    /// 自动判定冷却
    /// </summary>
    /// <param name="deltaTime"></param>
    public void OnUpdate(float deltaTime)
    {
        attackTimer -= deltaTime;
        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = finalInterval;
        }
    }

    protected abstract void Attack();



    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
    public void ApplyBonus(UpgradeData upgrade)
    {
        //damageBonus += upgrade.damageBonus;
        //attackSpeedBonus += upgrade.attackSpeedBonus;
        //projectileBonus += upgrade.projectileBonus;
        //pierceBonus += upgrade.pierceBonus;
        //bulletSpeedBonus += upgrade.bulletSpeedBonus;
        //bulletSizeBonus += upgrade.bulletSizeBonus;
        //rangeBonus += upgrade.rangeBonus;
    }

    /// <summary>
    /// 从另一把武器继承部分加成（用于置换）
    /// </summary>
    public void InheritFrom(WeaponInstance other)
    {
        //level = Mathf.Max(1, other.level - 1);
        //damageBonus = other.damageBonus * 0.5f;
        //attackSpeedBonus = other.attackSpeedBonus * 0.5f;
        //projectileBonus = Mathf.FloorToInt(other.projectileBonus * 0.5f);
        //pierceBonus = Mathf.FloorToInt(other.pierceBonus * 0.5f);
        //bulletSpeedBonus = other.bulletSpeedBonus * 0.5f;
        //bulletSizeBonus = other.bulletSizeBonus * 0.5f;
        //rangeBonus = other.rangeBonus * 0.5f;
    }

}