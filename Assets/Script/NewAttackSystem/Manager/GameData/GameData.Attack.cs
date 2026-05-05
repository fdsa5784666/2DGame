// GameData.Combat.cs
using UnityEngine;

public partial class GameData : MonoBehaviour
{
    [Header("=== 基础暴击相关及羁绊加成（作用全局） ===")]
    [SerializeField] private float globalCriticalRate = 0.05f;
    [SerializeField] private float globalCriticalDamage = 1.5f;

    [SerializeField] private float criticalRateBonus = 0f;
    [SerializeField] private float criticalDamageBonus = 0f;
    [SerializeField] private float globalDamageBonus = 0f;
    [SerializeField] private float globalAttackSpeedBonus = 0f;      //减少的数值
    [SerializeField] private float maxHealthBonus = 0f;


    [Header("=== 子弹基础配置 ===")]
    [SerializeField] private BulletBaseData bulletSO;  // 拖入 SO

    [Header("=== 百分比加成 ===")]
    [SerializeField] private float attackPercentBonus = 0f;
    [SerializeField] private float bulletSpeedBonus = 0f;
    [SerializeField] private float bulletSizeBonus = 0f;

    [Header("=== 基础数值 ===")]
    [SerializeField] private float baseAttackSpeed = 0.5f;

    [Header("=== 固定值加成 ===")]
    [SerializeField] private float attackFlatBonus = 0f;
    [SerializeField] private int projectileCountBonus = 0;
    [SerializeField] private int pierceBonus = 0;
    [SerializeField] private int splitBonus = 0;
    [SerializeField] private float explosionRadiusBonus = 0f;

    // ========== 计算属性（基础 + 加成） ==========
    public float FinalAttack => (bulletSO.damage + attackFlatBonus) * (1 + attackPercentBonus) * (1 + globalDamageBonus);

    public float FinalBulletSpeed
    {
        get
        {
            if (bulletSO == null) return 8f;
            return bulletSO.speed * (1 + bulletSpeedBonus);
        }
    }

    public float FinalBulletSize
    {
        get
        {
            if (bulletSO == null) return 1f;
            return bulletSO.size * (1 + bulletSizeBonus);
        }
    }

    public int FinalPierceCount
    {
        get
        {
            if (bulletSO == null) return 1;
            return bulletSO.pierceCount + pierceBonus;
        }
    }

    public int FinalSplitCount  
    {
        get
        {
            if (bulletSO == null) return 0;
            return bulletSO.splitCount + splitBonus;
        }
    }

    public float FinalExplosionRadius
    {
        get
        {
            if (bulletSO == null) return 0f;
            return bulletSO.explosionRadius + explosionRadiusBonus;
        }
    }
    public float FinalCriticalRate => Mathf.Clamp01(globalCriticalRate + criticalRateBonus);
    public float FinalCriticalDamage => globalCriticalDamage + criticalDamageBonus;
    public float GlobalDamageBonus => globalDamageBonus;
    public int FinalProjectileCount => 1 + projectileCountBonus;
    public float FinalAttackSpeed => baseAttackSpeed * (1f - globalAttackSpeedBonus);
    public float FinalMaxHealth => baseMaxHealth * (1f + maxHealthBonus);

    public void ResetSynergyBonuses()
    {
        criticalRateBonus = 0f;
        criticalDamageBonus = 0f;
        globalDamageBonus = 0f;
        globalAttackSpeedBonus = 0f;
        maxHealthBonus = 0f;
    }
    // ========== 创建子弹信息快照 ==========
    public BulletInfo CreateBulletInfo(float skillMultiplier = 1f)
    {
        if (bulletSO == null)
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
            lifeTime = bulletSO.lifeTime,

            pierceCount = FinalPierceCount,
            homing = bulletSO.homing,
            homingStrength = bulletSO.homingStrength,
            splitCount = FinalSplitCount,
            splitAngle = bulletSO.splitAngle,
            explosionRadius = FinalExplosionRadius,
            explosionDamageMultiplier = bulletSO.explosionDamageMultiplier,

            bulletColor = bulletSO.bulletColor,
            hitEffectPrefab = bulletSO.hitEffectPrefab,
            trailEffectPrefab = bulletSO.trailEffectPrefab,

            isCritical = isCrit
        };
    }

    private float CalculateDamage(float skillMultiplier, out bool isCrit)
    {
        float baseDmg = FinalAttack * skillMultiplier;
        isCrit = Random.value < FinalCriticalRate;
        float critMulti = isCrit ? FinalCriticalDamage : 1f;

        float damage = baseDmg * critMulti;
        damage *= Random.Range(0.95f, 1.05f);

        return Mathf.Max(1f, Mathf.Round(damage));
    }
   

    // ========== 加成修改方法 ==========
    public void AddAttackPercent(float v) => attackPercentBonus += v;
    public void AddAttackSpeedBonus(float v) => globalAttackSpeedBonus += v; 
    public void AddBulletSpeed(float v) => bulletSpeedBonus += v;
    public void AddBulletSize(float v) => bulletSizeBonus += v;
    public void AddPierce(int v) => pierceBonus += v;
    public void AddProjectileCount(int v) => projectileCountBonus += v;
    public void AddSplitCount(int v) => splitBonus += v;
    public void AddExplosionRadius(float v) => explosionRadiusBonus += v;
    public void AddCriticalRate(float v) => criticalRateBonus = Mathf.Clamp(criticalRateBonus + v, 0, 1 - globalCriticalRate);
    public void AddCriticalDamage(float v) => criticalDamageBonus += v;
    public void AddDamageMultiplier(float v) => globalDamageBonus += v;
    public void AddMaxHealthBonus(float v) => maxHealthBonus += v;
    public void AddAttackFlat(float v) => attackFlatBonus += v;


}