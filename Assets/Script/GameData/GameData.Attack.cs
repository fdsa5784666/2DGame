// GameData.Combat.cs
using UnityEngine;

public partial class GameData : MonoBehaviour
{
    [Header("=== 玩家通用战斗属性 ===")]
    [SerializeField] private float baseCriticalRate = 0.05f;
    [SerializeField] private float baseCriticalDamage = 1.5f;
    [SerializeField] private float criticalRateBonus = 0f;
    [SerializeField] private float criticalDamageBonus = 0f;

    [Header("=== 子弹基础配置 ===")]
    [SerializeField] private BulletBaseData bulletBaseData;  // 拖入 SO

    [Header("=== 攻击力基础 ===")]
    [SerializeField] private float baseAttack = 10f;

    [Header("=== 百分比加成（运行时动态修改）===")]
    [SerializeField] private float attackPercentBonus = 0f;
    [SerializeField] private float bulletSpeedBonus = 0f;
    [SerializeField] private float bulletSizeBonus = 0f;
    [SerializeField] private float globalDamageMultiplier = 1f;

    [Header("=== 固定值加成 ===")]
    [SerializeField] private float attackFlatBonus = 0f;
    [SerializeField] private int projectileCountBonus = 0;
    [SerializeField] private int pierceBonus = 0;
    [SerializeField] private int splitBonus = 0;
    [SerializeField] private float explosionRadiusBonus = 0f;

    // ========== 计算属性（基础 + 加成） ==========
    public float FinalAttack => (baseAttack + attackFlatBonus) * (1 + attackPercentBonus);

    public float FinalBulletSpeed
    {
        get
        {
            if (bulletBaseData == null) return 8f;
            return bulletBaseData.speed * (1 + bulletSpeedBonus);
        }
    }

    public float FinalBulletSize
    {
        get
        {
            if (bulletBaseData == null) return 1f;
            return bulletBaseData.size * (1 + bulletSizeBonus);
        }
    }

    public int FinalPierceCount
    {
        get
        {
            if (bulletBaseData == null) return 1;
            return bulletBaseData.pierceCount + pierceBonus;
        }
    }

    public int FinalSplitCount
    {
        get
        {
            if (bulletBaseData == null) return 0;
            return bulletBaseData.splitCount + splitBonus;
        }
    }

    public float FinalExplosionRadius
    {
        get
        {
            if (bulletBaseData == null) return 0f;
            return bulletBaseData.explosionRadius + explosionRadiusBonus;
        }
    }
    public float FinalCriticalRate => Mathf.Clamp01(baseCriticalRate + criticalRateBonus);
    public float FinalCriticalDamage => baseCriticalDamage + criticalDamageBonus;
    public float GlobalDamageMultiplier => globalDamageMultiplier;
    public int FinalProjectileCount => 1 + projectileCountBonus;

    // ========== 创建子弹信息快照 ==========
    public BulletInfo CreateBulletInfo(float skillMultiplier = 1f)
    {
        if (bulletBaseData == null)
        {
            Debug.LogError("BulletBaseData 未配置！请在 GameData 的 Inspector 中拖入 SO。");
            return new BulletInfo { damage = 10, speed = 8, size = 1, lifeTime = 3, pierceCount = 1 };
        }

        // 计算伤害
        float damage = CalculateDamage(skillMultiplier, out bool isCrit);

        return new BulletInfo
        {
            damage = damage,
            speed = FinalBulletSpeed,
            size = FinalBulletSize,
            lifeTime = bulletBaseData.lifeTime,

            pierceCount = FinalPierceCount,
            homing = bulletBaseData.homing,
            homingStrength = bulletBaseData.homingStrength,
            splitCount = FinalSplitCount,
            splitAngle = bulletBaseData.splitAngle,
            explosionRadius = FinalExplosionRadius,
            explosionDamageMultiplier = bulletBaseData.explosionDamageMultiplier,

            bulletColor = bulletBaseData.bulletColor,
            hitEffectPrefab = bulletBaseData.hitEffectPrefab,
            trailEffectPrefab = bulletBaseData.trailEffectPrefab,

            isCritical = isCrit
        };
    }

    private float CalculateDamage(float skillMultiplier, out bool isCrit)
    {
        float baseDmg = FinalAttack * skillMultiplier;
        isCrit = Random.value < FinalCriticalRate;
        float critMulti = isCrit ? FinalCriticalDamage : 1f;

        float damage = baseDmg * critMulti * globalDamageMultiplier;
        damage *= Random.Range(0.95f, 1.05f);

        return Mathf.Max(1f, Mathf.Round(damage));
    }

   

    // ========== 加成修改方法 ==========
    public void AddAttackPercent(float v) => attackPercentBonus += v;
    public void AddBulletSpeed(float v) => bulletSpeedBonus += v;
    public void AddBulletSize(float v) => bulletSizeBonus += v;
    public void AddPierce(int v) => pierceBonus += v;
    public void AddProjectileCount(int v) => projectileCountBonus += v;
    public void AddSplitCount(int v) => splitBonus += v;
    public void AddExplosionRadius(float v) => explosionRadiusBonus += v;
    public void AddCriticalRate(float v) => criticalRateBonus = Mathf.Clamp(criticalRateBonus + v, 0, 1 - baseCriticalRate);
    public void AddCriticalDamage(float v) => criticalDamageBonus += v;
    public void AddGlobalDamageMultiplier(float v) => globalDamageMultiplier += v;

}