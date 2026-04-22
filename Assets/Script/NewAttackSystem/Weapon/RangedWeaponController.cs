using UnityEngine;

/// <summary>
/// 远程武器控制器 - 发射子弹
/// </summary>
public class RangedWeaponController : WeaponController
{
    private GameObject bulletPrefab;
    private Transform firePoint;

    public override void Initialize(WeaponInstance weaponInstance, Transform player, LayerMask enemyLayers)
    {
        base.Initialize(weaponInstance, player, enemyLayers);

        // 获取子弹预制体
        bulletPrefab = weapon.Data.bulletPrefab;
        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullets/DefaultBullet");
        }

        // 获取或创建发射点
        firePoint = player.Find("FirePoint");
        if (firePoint == null)
        {
            var fp = new GameObject("FirePoint");
            fp.transform.SetParent(player);
            fp.transform.localPosition = Vector3.zero;
            firePoint = fp.transform;
        }
    }

    protected override void Attack()
    {
        int projectileCount = weapon.FinalProjectileCount;
        float spreadAngle = weapon.Data.spreadAngle;

        Vector2 baseDirection = GetAttackDirection();

        if (projectileCount == 1)
        {
            // 单发
            FireBullet(baseDirection, 0);
        }
        else
        {
            // 多发散射
            float startAngle = -spreadAngle / 2f;
            float angleStep = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0;

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + angleStep * i;
                FireBullet(baseDirection, angle);
            }
        }
    }

    private void FireBullet(Vector2 direction, float angleOffset)
    {
        Vector2 finalDirection = direction;
        if (angleOffset != 0)
        {
            finalDirection = Quaternion.Euler(0, 0, angleOffset) * direction;
        }

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, finalDirection);

        // 从对象池获取子弹
        GameObject bulletObj = ObjectPool.Instance?.SpawnFromPool(bulletPrefab.name, firePoint.position, rotation.eulerAngles.z)
                              ?? Object.Instantiate(bulletPrefab, firePoint.position, rotation);

        var bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(CreateBulletInfo());
        }

        // 应用命中效果
        //if (weapon.Data.onHitEffect != EOnHitEffect.None)
        //{
        //    bullet?.SetOnHitEffect(weapon.Data.onHitEffect, weapon.Data.effectChance,
        //                           weapon.Data.effectDuration, weapon.Data.effectPower);
        //}
    }
}