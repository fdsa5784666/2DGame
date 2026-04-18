using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoLock : MonoBehaviour
{
    private float lockRange = 0f;

    private Transform currentTarget = null;
    private float lastFindTargetTime = 0f;
    private float findTargetInterval = 0.5f;


    private void Update()
    {
        if (Time.time - lastFindTargetTime > findTargetInterval)
        {
            lockRange = GameData.Instance.attackRange;

            FindClosestEnemy();
            lastFindTargetTime = Time.time;
        }
    }

    void FindClosestEnemy()
    {
        float closestDistance = lockRange;
        Transform closestEnemy = null;

        foreach (Enemy enemy in Enemy.ActiveEnemies)
        {
            if (enemy == null)
            {
                GameData.Instance.isLockEnemy = true;
                return;
            }


            GameData.Instance.isLockEnemy = true;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {

                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        currentTarget = closestEnemy;


    }

    public Transform GetCurrentTarget() => currentTarget;
}