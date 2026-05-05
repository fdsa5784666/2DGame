//using UnityEngine;
//using System;

//public class EnemyHealth : MonoBehaviour, IDamageable
//{
//    [Header("=== 基础属性 ===")]
//    [SerializeField] private float maxHealth = 50f;
//    [SerializeField] private ETeam team = ETeam.Enemy;

//    [Header("=== 掉落 ===")]
//    [SerializeField] private int expValue = 10;
//    [SerializeField] private int goldValue = 5;
//    [SerializeField] private GameObject deathEffectPrefab;

//    [Header("=== 受击反馈 ===")]
//    [SerializeField] private float hitFlashDuration = 0.1f;
//    [SerializeField] private Color hitFlashColor = Color.red;

//    // 状态
//    private float currentHealth;
//    private bool isAlive = true;

//    // 组件
//    private SpriteRenderer spriteRenderer;
//    private Color originalColor;

//    // 事件
//    public event Action<float, float> OnHealthChanged;  // 当前血量, 最大血量
//    public event Action<float, bool> OnDamaged;         // 伤害值, 是否暴击
//    public event Action OnDeath;

//    // 运行时倍率（用于难度调整）
//    private float healthMultiplier = 1f;
//    private float damageMultiplier = 1f;

//    void Awake()
//    {
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        if (spriteRenderer != null)
//        {
//            originalColor = spriteRenderer.color;
//        }
//    }

//    void Start()
//    {
//        currentHealth = maxHealth * healthMultiplier;
//    }

//    #region IDamageable 实现
//    public void TakeDamage(float damage, bool isCrit = false)
//    {
//        if (!isAlive) return;

//        // 应用伤害倍率（如果是敌人对玩家造成伤害时使用）
//        float finalDamage = damage;

//        currentHealth -= finalDamage;
//        OnHealthChanged?.Invoke(currentHealth, maxHealth * healthMultiplier);
//        OnDamaged?.Invoke(finalDamage, isCrit);

//        // 受击反馈
//        if (finalDamage > 0)
//        {
//            HitFeedback(finalDamage, isCrit);
//        }

//        // 死亡检查
//        if (currentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    public Transform GetTransform() => transform;

//    public bool IsAlive() => isAlive;

//    public ETeam GetTeam() => team;
//    #endregion

//    #region 受击反馈
//    private void HitFeedback(float damage, bool isCrit)
//    {
//        // 伤害数字
//        //DamagePopup.Create(transform.position, damage, isCrit);

//        // 闪白效果
//        //if (spriteRenderer != null)
//        //{
//        //    StopAllCoroutines();
//        //    StartCoroutine(FlashRoutine());
//        //}

//        // 相机震动（暴击时更强）
//        //float shakeMagnitude = isCrit ? 0.3f : 0.1f;
//        //CameraShake.Instance?.Shake(0.1f, shakeMagnitude);
//    }

//    private System.Collections.IEnumerator FlashRoutine()
//    {
//        spriteRenderer.color = hitFlashColor;
//        yield return new WaitForSeconds(hitFlashDuration);
//        spriteRenderer.color = originalColor;
//    }
//    #endregion

//    #region 死亡处理
//    private void Die()
//    {
//        if (!isAlive) return;

//        isAlive = false;
//        OnDeath?.Invoke();

//        // 死亡特效
//        if (deathEffectPrefab != null)
//        {
//            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
//        }

//        // 掉落奖励
//        DropRewards();

//        // 通知游戏管理器
//        GameData.Instance?.AddKill();
//        GameData.Instance?.AddExp(expValue);
//        GameData.Instance?.AddGold(goldValue);

//        // 通知敌人生成器
//        EnemySpawner.Instance?.OnEnemyKilled(gameObject);

//        // 销毁或回池
//        Destroy(gameObject);
//    }

//    private void DropRewards()
//    {
//        // 经验球
//        if (expValue > 0)
//        {
//            // 生成经验球逻辑
//        }

//        // 金币
//        if (goldValue > 0)
//        {
//            // 生成金币逻辑
//        }
//    }
//    #endregion

//    #region 公共方法
//    /// <summary>
//    /// 设置难度倍率
//    /// </summary>
//    public void SetMultipliers(float healthMult, float damageMult, float speedMult)
//    {
//        healthMultiplier = healthMult;
//        damageMultiplier = damageMult;

//        // 更新当前血量（按比例调整）
//        float healthPercent = currentHealth / (maxHealth * (healthMult / healthMultiplier));
//        maxHealth *= healthMult;
//        currentHealth = maxHealth * healthPercent;
//    }

//    /// <summary>
//    /// 治疗
//    /// </summary>
//    public void Heal(float amount)
//    {
//        if (!isAlive) return;

//        currentHealth = Mathf.Min(maxHealth * healthMultiplier, currentHealth + amount);
//        OnHealthChanged?.Invoke(currentHealth, maxHealth * healthMultiplier);

//        //DamagePopup.Create(transform.position, amount, false, DamageType.Heal);
//    }

//    /// <summary>
//    /// 获取当前血量百分比
//    /// </summary>
//    public float GetHealthPercent()
//    {
//        return currentHealth / (maxHealth * healthMultiplier);
//    }
//    #endregion
//}