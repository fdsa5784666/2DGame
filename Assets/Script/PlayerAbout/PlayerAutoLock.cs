//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 此自动锁定方法是在敌人从对象池取出来就存入在场对象列表
///// 需要时遍历查找范围内最近
///// 
///// 还有一个 那个不是单独类 故在此提一嘴
///// 那个是每次都用OverLapCircle（可能不是这么拼）找锁定范围内最近
///// </summary>
//public class PlayerAutoLock : MonoBehaviour
//{
//    private float lockRange = 5f;
//    private Transform currentTarget = null;
//    public Transform CurrentTarget => currentTarget;

//    private float lastFindTargetTime = 0f;
//    private float findTargetInterval = 0.1f;


//    private void Update()
//    {
//        if (GameData.Instance != null && GameData.Instance.CurrentState != EGameState.Playing) return;

//        if (Time.time - lastFindTargetTime > findTargetInterval && AttackManager.Instance.AutoAttack)
//        {
//            lockRange = GameData.Instance.attackRange;

//            FindClosestEnemy();
//            lastFindTargetTime = Time.time;
//        }
//    }

//    void FindClosestEnemy()
//    {
//        float closestDistance = lockRange;
//        Transform closestEnemy = null;

//        foreach (Enemy enemy in Enemy.ActiveEnemies)
//        {
//            if (enemy == null)
//            {
//                GameData.Instance.isLockEnemy = true;
//                return;
//            }


//            GameData.Instance.isLockEnemy = true;
//            float distance = Vector2.Distance(transform.position, enemy.transform.position);
//            if (distance < closestDistance)
//            {

//                closestDistance = distance;
//                closestEnemy = enemy.transform;
//            }
//        }

//        currentTarget = closestEnemy;


//    }
//}