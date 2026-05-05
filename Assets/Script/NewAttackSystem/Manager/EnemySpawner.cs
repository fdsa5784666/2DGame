using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [System.Serializable]
    public class WaveConfig
    {
        public string waveName;
        public float startTime;               // 波次开始时间
        public float endTime;                 // 波次结束时间
        public string enemyTag;               // 生成敌人类型标签(同对象池名)
        public float baseSpawnRate = 1f;          // 基础的每秒生成数量
        [HideInInspector]
        public float spawnRate = 1f;              // 实际生成速率（受曲线影响）
        public AnimationCurve spawnRateCurve; // 生成速率随时间变化的曲线
        public int maxActiveCount = 50;       // 同时存在最多数
        public float healthMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;
    }

    [Header("=== 波次配置 ===")]
    [SerializeField] private List<WaveConfig> waves = new List<WaveConfig>();

    [Header("=== Boss配置 ===")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private float bossSpawnTime = 1140f; // 19分钟
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private float bossHealthMultiplier = 5f;

    [Header("=== 生成参数 ===")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(20, 15);
    [SerializeField] private float spawnDistanceFromPlayer = 8f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("=== 引用 ===")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ObjectPool _pool;
    [SerializeField] private Camera mainCamera;

    // 运行时状态
    [SerializeField]private float gameTimer = 0f;
    private bool isEndlessMode = false;
    private bool bossSpawned = false;
    private float endlessDifficultyMultiplier = 1f;
    private Dictionary<WaveConfig, float> waveSpawnTimers;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // 属性
    public int ActiveEnemyCount => activeEnemies.Count;
    public bool IsEndlessMode => isEndlessMode;

    // 事件
    public event System.Action<GameObject> OnEnemySpawned;
    public event System.Action<GameObject> OnEnemyDeath;
    public event System.Action OnBossSpawned;
    public event System.Action OnBossDefeated;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        waveSpawnTimers = new Dictionary<WaveConfig, float>();
        foreach (var wave in waves)
        {
            waveSpawnTimers[wave] = 0f;
        }
    }

    void Start()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        
        if(mainCamera == null)
            mainCamera = Camera.main;   
    }

    void Update()
    {
        if (GameData.Instance != null && GameData.Instance.CurrentState != EGameState.Playing)
            return;

        // 按时间生成敌人
        foreach (var wave in waves)
        {
            if (gameTimer < wave.startTime || gameTimer > wave.endTime)
                continue;

            if (!waveSpawnTimers.ContainsKey(wave))
                waveSpawnTimers[wave] = 0f;

            waveSpawnTimers[wave] -= Time.deltaTime;
            wave.spawnRate = wave.baseSpawnRate * 
                wave.spawnRateCurve.Evaluate((gameTimer - wave.startTime) / (wave.endTime - wave.startTime));
            if (waveSpawnTimers[wave] <= 0 && activeEnemies.Count < wave.maxActiveCount)
            {
                Debug.Log("当前可生成敌人，尝试生成...");
                SpawnEnemy(wave);
                waveSpawnTimers[wave] = 1f / wave.spawnRate;
            }
        }

        gameTimer += Time.deltaTime;

        // Boss生成检查
        if (!bossSpawned && gameTimer >= bossSpawnTime)
        {
            OnBossSpawned?.Invoke();
            //SpawnBoss();
        }
        
    }

    #region 生成逻辑

    private void SpawnEnemy(WaveConfig wave)
    {
        // 设置位置
        Vector2 spawnPos = GetValidSpawnPosition(2f);

        GameObject enemy = _pool.SpawnFromPool(wave.enemyTag, spawnPos);
        activeEnemies.Add(enemy);
        OnEnemySpawned?.Invoke(enemy);
    }

    private Vector2 GetValidSpawnPosition(float extraDistance = 1f)
    {
        //if (playerTransform == null)
        //    return GetRandomPositionInArea();

        //for (int i = 0; i < 30; i++)
        //{
        //    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        //    Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnDistanceFromPlayer;
        //    Vector2 spawnPos = (Vector2)playerTransform.position + offset;

        //    // 确保在生成区域内
        //    spawnPos.x = Mathf.Clamp(spawnPos.x, -spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        //    spawnPos.y = Mathf.Clamp(spawnPos.y, -spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        //    // 检查是否有障碍物
        //    if (!Physics2D.OverlapCircle(spawnPos, 1f, obstacleLayer))
        //    {
        //        return spawnPos;
        //    }
        //}

        //return GetRandomPositionInArea();
        // 获取屏幕边界的世界坐标
        Vector2 min = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector2 max = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // 随机选择一条边
        int edge = Random.Range(0, 4);

        float x = 0, y = 0;

        switch (edge)
        {
            case 0: // 左边
                x = min.x - extraDistance;
                y = Random.Range(min.y, max.y);
                break;
            case 1: // 右边
                x = max.x + extraDistance;
                y = Random.Range(min.y, max.y);
                break;
            case 2: // 下边
                x = Random.Range(min.x, max.x);
                y = min.y - extraDistance;
                break;
            case 3: // 上边
                x = Random.Range(min.x, max.x);
                y = max.y + extraDistance;
                break;
        }

        return new Vector2(x, y);
    }

    private Vector2 GetRandomPositionInArea()
    {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float y = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return new Vector2(x, y);
    }
    #endregion

    //#region Boss
    //private void SpawnBoss()
    //{
    //    bossSpawned = true;

    //    Vector2 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : GetRandomPositionInArea();

    //    var boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity, enemiesContainer);

    //    var bossBase = boss.GetComponent<Enemy>();
    //    if (bossBase != null)
    //    {
    //        bossBase.SetMultipliers(bossHealthMultiplier * endlessDifficultyMultiplier,
    //                                endlessDifficultyMultiplier, 1f);
    //        bossBase.OnDeath += OnBossDeath;
    //    }

    //    activeEnemies.Add(boss);
    //    OnEnemySpawned?.Invoke(boss);
    //    OnBossSpawned?.Invoke();

    //    Debug.Log("Boss已生成！");
    //}

    //private void OnBossDeath(Enemy boss)
    //{
    //    boss.OnDeath -= OnBossDeath;
    //    OnBossDefeated?.Invoke();

    //    if (GameManager.Instance != null && GameManager.Instance.CurrentMode == GameMode.Standard)
    //    {
    //        GameManager.Instance.Victory();
    //    }
    //}
    //#endregion

    #region 公共方法
    /// <summary>
    /// 每个怪物死亡时调用，负责从activeEnemies列表中移除，并触发OnEnemyDeath事件
    /// </summary>
    /// <param name="enemy"></param>
    public void OnEnemyKilled(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        OnEnemyDeath?.Invoke(enemy);
    }
    public void OnBossDeath()
    {
        OnBossDefeated?.Invoke();  
    }
    //public void SetEndlessMode(bool enabled)
    //{
    //    isEndlessMode = enabled;
    //    endlessDifficultyMultiplier = enabled ? 1.5f : 1f;
    //}

    public List<GameObject> GetActiveEnemies()
    {
        return new List<GameObject>(activeEnemies);
    }

    public int GetActiveEnemyCount() => activeEnemies.Count;

    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
    }

    public void　ResetEnemySpawner()
    {
        gameTimer = 0f;
        bossSpawned = false;
        if (waves != null || waveSpawnTimers != null)
        {
            foreach (var wave in waves)
            {
                waveSpawnTimers[wave] = 0f;
            }
        }
        
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                OnEnemyDeath?.Invoke(enemy);
                TriangleMonster triangle = enemy.GetComponent<TriangleMonster>();
                if (triangle != null)
                    triangle.ReturnToPool();
            }
        }
        activeEnemies.Clear();
    }
    #endregion

    #region 编辑器
    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(Vector3.zero, spawnAreaSize);

        //if (playerTransform != null)
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawWireSphere(playerTransform.position, spawnDistanceFromPlayer);
        //}

        //if (bossSpawnPoint != null)
        //{
        //    Gizmos.color = Color.magenta;
        //    Gizmos.DrawWireSphere(bossSpawnPoint.position, 1f);
        //}
    }
    #endregion
}