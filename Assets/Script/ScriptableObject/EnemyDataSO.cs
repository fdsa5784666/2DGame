using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData",menuName ="ScriptableObjects/Enemy Base Data")]
public class EnemyDataSO : ScriptableObject
{
    public string enemyName;
    public float health;
    public float speed;
    public int damage;
}
