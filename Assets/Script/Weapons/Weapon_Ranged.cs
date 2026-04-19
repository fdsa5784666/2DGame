using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Weapon_Ranged : WeaponBase
{
    [Header("=== 射击参数 ===")]
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private bool _autoTarget = true;

    private Transform currentTarget;
    protected override void Attack()
    {
        if(_bulletPrefab == null)
        {
            Debug.LogWarning("子弹预制体未设置");
            return;
        }

        Vector3 direction;

        int bulletCount = FinalProjectileCount;
        float totalSpread = weaponData.spreadAngle;
        float startAngle = -totalSpread / 2f;
        float angleStep = bulletCount >1 ? totalSpread/(bulletCount - 1) : 0f;

        
    }
    public override void Initialize(Transform player)
    {
        base.Initialize(player);
    }

    protected override void ApplyUpgrade(WeaponUpgradeData upgrade)
    {
        base.ApplyUpgrade(upgrade);
    }



    protected override float CalculateFinalDamage()
    {
        return base.CalculateFinalDamage();
    }

    protected override BulletInfo CreateBulletInfo()
    {
        return base.CreateBulletInfo();
    }


}
