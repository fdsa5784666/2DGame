using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("=== 武器数据 ===")]
    [SerializeField]
    protected WeaponData weaponData;

    //运行时状态
    protected float attackTimer = 0f;
    protected bool canAttack = true;
    protected Transform playerTransform;

    public string WeaponName => weaponData?.weaponName ?? "Unknown Weapon";
    public WeaponData Data => weaponData;

    public float FinalDamage
    {
        get
        {
            if (weaponData == null) return 10f;
            return weaponData.baseDamage * (1 + weaponData.damagePercentBonus);
        }
    }
    public float FinalAttackSpeed
    {
        get
        {
            if (weaponData == null) return 1f;
            return weaponData.attackSpeed * (1 + weaponData.attackSpeedBonus);
        }
    }
    public int FinalProjectileCount
    {
        get
        {
            if (weaponData == null) return 1;
            return weaponData.projectileCount * (1 + weaponData.projectileCountBonus);
        }

    }

    public virtual void Initialize(Transform player)
    {
        playerTransform = player;
        attackTimer = 0f;
        canAttack = true;
    }

    protected virtual void Update()
    {
        if (!canAttack || playerTransform == null) return;
        
        attackTimer += Time.deltaTime;
        float attackInterval = 1f / FinalAttackSpeed;

        if (attackTimer >= attackInterval)
        {
            attackTimer -= attackInterval;
            Attack();

        }
        
    }
    protected abstract void Attack();

    protected virtual BulletInfo CreateBulletInfo()
    {
        if(weaponData?.bulletData == null)
        {
            Debug.LogError("武器或子弹数据未设置");
            return new BulletInfo();
        }

        BulletBaseData bullet = weaponData.bulletData;
        float damage = CalculateFinalDamage();

        return new BulletInfo()
        {
            damage = damage,
            speed = bullet.speed * (1 + weaponData.bulletSpeedBonus),
            size = bullet.size * (1 + weaponData.bulletSizeBonus),
            lifetime = bullet.lifeTime,

            pierceCount = bullet.pierceCount + weaponData.pierceBonus,
            homing = bullet.homing,
            homingStrength = bullet.homingStrength,
            splitCount = bullet.splitCount + weaponData.splitBonus,
            splitAngle = bullet.splitAngle,
            explosionRadius = bullet.explosionRadius + weaponData.explosionRadiusBonus,
            explosionDamageMultiplier = bullet.explosionDamageMultiplier,

            bulletColor = bullet.bulletColor,
            hitEffectPrefab = bullet.hitEffectPrefab,
            trailEffectPrefab = bullet.trailEffectPrefab,

            isCritical = Random.value < GameData.Instance.FinalCriticalRate
        };
    }

    protected virtual float CalculateFinalDamage()
    {
        float baseDmg = FinalDamage * weaponData.skillMultiplier;

        bool isCrit = Random.value < GameData.Instance.FinalCriticalRate;
        float critMulti = isCrit ? GameData.Instance.FinalCriticalDamage : 1f;

        float damage = baseDmg * critMulti * GameData.Instance.GlobalDamageMultiplier;
        damage *= Random.Range(0.95f, 1.05f);

        return Mathf.Max(1f, Mathf.Round(damage));
    }

    protected virtual void ApplyUpgrade(WeaponUpgradeData upgrade)
    {
        if (upgrade.targetWeaponName != weaponData.weaponName) return;

        weaponData.damagePercentBonus += upgrade.damagePercentBonus;
        weaponData.attackSpeedBonus += upgrade.attackSpeedBonus;
        weaponData.projectileCountBonus += upgrade.projectileCountBonus;
        weaponData.bulletSpeedBonus += upgrade.bulletSpeedBonus;
        weaponData.bulletSizeBonus += upgrade.bulletSizeBonus;
        weaponData.pierceBonus += upgrade.pierceBonus;
        weaponData.splitBonus += upgrade.splitBonus;
        weaponData.explosionRadiusBonus += upgrade.explosionRadiusBonus;
    }

    public void SetEnabled(bool enabled) => canAttack = enabled;
}
