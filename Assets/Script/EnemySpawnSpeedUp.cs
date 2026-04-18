using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSpeedUp : MonoBehaviour
{

    private void Awake()
    {
        InvokeRepeating("EnemySpawnAccelerate", 50, 50);
    }

    void EnemySpawnAccelerate()
    {
        float temp = GameData.Instance.spawnInterval - 0.5f;
        GameData.Instance.spawnInterval = Mathf.Clamp(temp, 0.4f, 5);
    }
}
