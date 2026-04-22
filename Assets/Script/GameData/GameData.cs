using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData : MonoBehaviour
{
    public static GameData Instance;
    [Header("游戏中参数")]
    public int killedEnemy = 0;

    [Header("玩家战斗通用属性")]
    public float playerMaxHealth = 100f;
    public float playerCurrentHealth;
    public bool isLockEnemy = false;
    public int playerEXP = 0;
    public int playerLevel = 1;
    public int expToNextLevel = 10;
    public const float LevelUpValueMultValue = 1.5f;
    public const int playerFirstUpgradeValue = 10; 
    public float playerSpeed = 4f;

    [Header("子弹属性")]
    public float attackRange = 10f;
    public float bulletDamage = 10f;
    public float bulletSpeed = 5f;
    public float bulletLifeTime = 10f;
    //过多少秒可发射一次子弹
    public float shootInterval = 1f;
    public int shootScale = 1;
    //子弹是否可穿透
    public bool bulletPenetrable = false;

    [Header("怪物通用")]
    [SerializeField] private int enemyCollideDamage = 10;
    [SerializeField] private float enemyMaxHp = 10f;
    [SerializeField] private float repelMultValue = 1.5f;
    [SerializeField] private int maxActiveEnemy = 180;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float triangleMoveSpeed = 2f;
    [SerializeField] private int triangleMonsterEXP = 1;
    [SerializeField] private int triangleMonsterGold = 1;

    public int EnemyCollideDamage => enemyCollideDamage;
    public float EnemyMaxHp => enemyMaxHp;
    public float RepelMultValue => repelMultValue;
    public int MaxActiveEnemy => maxActiveEnemy;
    public float SpawnInterval => spawnInterval;
    public float TriangleMoveSpeed => triangleMoveSpeed;
    public int TriangleMonsterEXP => triangleMonsterEXP;
    public int TriangleMonsterGold => triangleMonsterGold;

    [Header("游戏逻辑参数")]
    public float HealthPotionRandomValue = 0.05f;

    [Header("游戏内预设体")]
    public GameObject HealthPotionPrefab;

    public GameObject PauseSignal;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }
    private void Update()
    {
        if (playerMaxHealth >= 10000000)
        {
            playerMaxHealth = 9999999;
        }
    }

    public void FullHP ()
    {
        playerCurrentHealth = playerMaxHealth;
    }
    /// <summary>
    /// 将时间状态倒置 即暂停情况下恢复 常规情况下暂停
    /// Pause and Restore
    /// </summary>
    public void Pause()
    {
        Time.timeScale = Time.timeScale > 0 ? 0 : 1.0f;
        PauseSignal.SetActive(!PauseSignal.activeSelf);
    }
}
