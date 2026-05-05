using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 继承基类 额外实现查找最近敌人逻辑
/// </summary>
public class RangedWeapon : WeaponInstance
{
    private Enemy target;
    protected float lockInterval = 0.1f;
    protected float lockTimer = 0f;

    private void Update()
    {
        target = FindNearestEnemy();
        OnUpdate(Time.deltaTime);
    }
    public override void Initialize(WeaponData data, Transform attackPoint,int starLevel = 1)
    {
        base.Initialize(data, attackPoint, starLevel);
    }

    public override void RefreshStats()
    {
        base.RefreshStats();
    }
    /// <summary>
    /// 发射子弹 直线飞行
    /// </summary>
    protected override void Attack()
    {
        if (target == null) return;
        //Debug.Log($"攻击目标");
        //生成子弹 发射子弹
        GameObject bullet = ObjectPool.Instance.SpawnFromPool("Bullet", attackPoint.position);
        Bullet bulletS = bullet.GetComponent<Bullet>();
        bulletS.Initialize(GameData.Instance.CreateBulletInfo());

        Vector2 direction = (target.transform.position - attackPoint.position).normalized;
        bulletS.SetDirection(direction);
        //Debug.Log($"发射子弹向{direction}");
    }
    public override void Destroy()
    {
        base.Destroy();
    }
    private Enemy FindNearestEnemy()
    {
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;
        //Debug.Log("=== 开始查找最近敌人 ===");

        foreach (Enemy enemy in Enemy.ActiveEnemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(attackPoint.position, enemy.transform.position);

            bool inRange = distance <= GameData.Instance.attackRange;

            if (distance < closestDistance && distance < GameData.Instance.attackRange)
            {

                closestDistance = distance;
                closestEnemy = enemy;
            }

        }

        //GameData.Instance.isLockEnemy = (closestEnemy != null);
        return closestEnemy;
    }
    //    private Enemy FindNearestEnemy()
    //    {
    //        Enemy nearest = null;
    //        float nearestDist = float.MaxValue;

    //        List<Enemy> enemies = Enemy.ActiveEnemies;

    //        for (int i = 0; i < enemies.Count; i++)
    //        {
    //            Enemy e = enemies[i];
    //            if (e == null) continue;

    //            float dist = Vector2.Distance(transform.position, e.transform.position);

    //            if (dist < nearestDist)
    //            {
    //                nearestDist = dist;
    //                nearest = e;
    //            }
    //        }
    //        if (nearest != null)
    //        {
    //            Debug.Log($"玩家位置:{transform.position} 敌人位置:{nearest.transform.position} 距离:{nearestDist:F2}");
    //            if (nearestDist < 3f)
    //                Debug.LogError($"敌人距离{nearestDist:F2} 却没攻击？ attackRange={GameData.Instance.attackRange}");
    //        }

    //        // 强制输出每一帧锁的是谁
    //        if (nearest != null)
    //        Debug.Log($"锁定目标:{nearest.name} id:{nearest.GetInstanceID()}  距离:{nearestDist:F2}");
    //        return nearest;
    //    }
}
