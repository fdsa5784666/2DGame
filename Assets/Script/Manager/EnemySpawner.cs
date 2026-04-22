using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class EnemySpawner : MonoBehaviour
//{
//    private Vector3 spawnPosition;
//    public GameObject EnemyPrefab;
//    private float spawnOffset = 2f;

//    private Camera mainCamera;
//    private void Awake()
//    {
//        mainCamera = Camera.main;
//        InvokeRepeating(nameof(SpawnEnemy), 4f, GameData.Instance.spawnInterval);
//    }
//    void SpawnEnemy()
//    {
//        if (Enemy.ActiveEnemies.Count > GameData.Instance.maxActiveEnemy)
//        {
//            Debug.Log("在场敌人已达上限");
//            return;

//        }

//        spawnPosition = GetRandomPosition();
//        ObjectPool.Instance.SpawnFromPool("TriangleMonster", spawnPosition);
//    }
//    /// <summary>
//    /// 获取随机屏幕外位置 再生成在指定位置
//    /// </summary>
//    /// <returns></returns>
//    Vector3 GetRandomPosition()
//    {
//        Vector2 min = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
//        Vector2 max = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

//        min -= Vector2.one * spawnOffset;
//        max += Vector2.one * spawnOffset;

//        float x = Random.Range(min.x, max.x);
//        float y = Random.Range(min.y, max.y);

//        Vector2 screenMin = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
//        Vector2 screenMax = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

//        if (x > screenMin.x && x < screenMax.x && y > screenMin.y && y < screenMax.y) 
//        {
//            float distToLeft = Mathf.Abs(x - screenMin.x);
//            float distToRight = Mathf.Abs(screenMax.x - x);
//            float distToBottom = Mathf.Abs(y - screenMin.y);
//            float distToTop = Mathf.Abs(screenMax.y - y);

//            float minDist = Mathf.Min(distToLeft, distToRight, distToBottom, distToTop);

//            if (minDist == distToLeft) x = screenMin.x - spawnOffset;
//            else if (minDist == distToRight) x = screenMax.x + spawnOffset;
//            else if(minDist == distToBottom) y = screenMin.y - spawnOffset;
//            else if (minDist == distToTop) y = screenMax.y + spawnOffset;
//        }
//        return new Vector3 (x, y, 0);
//    }
//}
