using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GPUDriven;

public partial class GameData : MonoBehaviour
{
    public static GameData Instance;

    [Header("=== 游戏状态 ===")]
    [SerializeField] private EGameState currentState = EGameState.Paused;
    [SerializeField] private float gameTimer = 0f; 

    [Header("=== 模式配置 ===")]
    [SerializeField] private GameMode currentMode = GameMode.Standard;
    [SerializeField] private float standardModeDuration = 1200f; // 20分钟 = 1200秒

    [Header("=== 玩家战斗通用属性 ===")]
    private float baseMaxHealth = 100f;
    public float playerMaxHealth => GameData.Instance.FinalMaxHealth;
    public float playerCurrentHealth = 100f;
    //public bool isLockEnemy = false;
    public int playerEXP = 0;
    public int playerLevel = 1;
    public int expToNextLevel = 10;
    public const float LevelUpValueMultValue = 1.5f;
    public const int playerFirstUpgradeValue = 10;
    private float basePlayerSpeed = 4f;
    private float playerSpeed = 4f;
    public float pickupRange = 3f;
    public int attackRange = 10;

    public float PlayerSpeed => playerSpeed;
    [SerializeField]
    private CharacterData playerChosenCharacter = null;
    public CharacterData PlayerChosenCharacter => playerChosenCharacter;
    public void SetCharacter(CharacterData c)
    {
        playerChosenCharacter = c;
        if (c != null)
            OnCharacterChosen?.Invoke(true);
        else
            OnCharacterChosen?.Invoke(false);
    }

    [Header("=== 资源 ===")]
    [SerializeField] private int gold = 0;
    [SerializeField] private int totalKills = 0;
    public float expMultiplier = 1f;
    public float goldMultiplier = 1f;
    //[Header("子弹属性")]
    //public float attackRange = 10f;
    //public float bulletDamage = 10f;
    //public float bulletSpeed = 5f;
    //public float bulletLifeTime = 10f;
    ////过多少秒可发射一次子弹
    //public float shootInterval = 1f;
    //public int shootScale = 1;
    ////子弹是否可穿透
    //public bool bulletPenetrable = false;

    [Header("怪物通用")]
    [SerializeField] private int enemyCollideDamage = 10;
    [SerializeField] private float enemyMaxHp = 10f;
    [SerializeField] private float repelMultValue = 1.5f;
    [SerializeField] private int maxActiveEnemy = 180;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float triangleMoveSpeed = 2f;
    [SerializeField] private int triangleMonsterEXP = 1;
    [SerializeField] private int triangleMonsterGold = 1;


    [Header("游戏逻辑参数")]
    public float HealthPotionRandomValue = 0.05f;

    [Header("=== 引用 ===")]
    public GameObject player;
    public WeaponSlotManager slotManager;
    public EnemySpawner enemySpawner;
    public UpgradeManager upgradeManager;
    public UIManager uiManager;
    public GameObject HealthPotionPrefab;

    public WeaponData pistolData;

    public GameObject PauseSignal;
    #region *** 属性 ***
    public int Gold => gold;
    public int TotalKills => totalKills;
    public GameMode CurrentMode => currentMode;
    public float StandardModeDuration => standardModeDuration;
    public int EnemyCollideDamage => enemyCollideDamage;
    public float EnemyMaxHp => enemyMaxHp;
    public float RepelMultValue => repelMultValue;
    public int MaxActiveEnemy => maxActiveEnemy;
    public float SpawnInterval => spawnInterval;
    public float TriangleMoveSpeed => triangleMoveSpeed;
    public int TriangleMonsterEXP => triangleMonsterEXP;
    public int TriangleMonsterGold => triangleMonsterGold;
    public EGameState CurrentState => currentState;
    public float GameTimer => gameTimer;
    #endregion
    public List<GameObject> activeNonpooledObj;

    // 事件
    public event Action<float> OnTimerUpdated;
    public event Action<int> OnGoldChanged;
    //public event Action<int> OnKillsChanged;
    public event Action<float, float> OnHealthChanged;
    public event Action<int, int, int> OnExpChanged;
    public event Action<EGameState> OnStateChanged;
    public event Action<int> OnLevelUp;
    public event Action<bool> OnCharacterChosen;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeGame();
    }

    private void Start()
    {
    }
    private void Update()
    {
        if (currentState != EGameState.Playing) return;

        // 游戏计时
        gameTimer += Time.deltaTime;
        OnTimerUpdated?.Invoke(gameTimer);

        // 标准模式：检查是否到达20分钟
        if (currentMode == GameMode.Standard && gameTimer >= standardModeDuration)
        {
            Victory();
        }

    }
    public void InitializeGame()
    {

        gameTimer = 0f;
        playerLevel = 1;
        playerEXP = 0;
        expToNextLevel = playerFirstUpgradeValue;
        playerCurrentHealth = playerMaxHealth;
        gold = 0;
        totalKills = 0;
    }

    #region 金币与击杀
    public void AddGold(int amount)
    {
        int actualAmount = Mathf.RoundToInt(amount * goldMultiplier);
        gold += actualAmount;
        OnGoldChanged?.Invoke(gold);
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }

    public void AddKill(int kill = 0)
    {
        if (kill == 0)
            totalKills++;
        else
            totalKills += kill;
        //OnKillsChanged?.Invoke(totalKills);
    }
    #endregion

    public void FullHP ()
    {
        playerCurrentHealth = playerMaxHealth;
    }

    public void Pause()
    {
        currentState = EGameState.Paused;
        Time.timeScale = 0f;
        PauseSignal.SetActive(true);
        OnStateChanged?.Invoke(currentState);
    }
    public void Resume()
    {
        currentState = EGameState.Playing;
        Time.timeScale = 1.0f;
        PauseSignal.SetActive(false);
        OnStateChanged?.Invoke(currentState);
    }
    public void GameOver(bool victory = false)
    {
        currentState = victory ? EGameState.Victory : EGameState.GameOver;
        Pause();
        OnStateChanged?.Invoke(currentState);

        // 通知元进度管理器
        //MetaProgressionManager.Instance?.OnGameFinished(victory, gold, totalKills);

        //中途退出 按跳过结算返回标题界面处理
        if (!victory && playerCurrentHealth > 0)
        {
            Reward();
            SceneManager.Instance.SetSceneType(ESceneType.Title);
            return;
        }

        //推测也要有胜利失败两种情况
        uiManager?.ShowGameOver(victory);
    }

    public void Victory()
    {
        GameOver(true);
    }

    public void Reward()
    {
        SaveManager.Instance.AddRewardOnGameEnd(Gold, TotalKills);

        Debug.Log($"gold:{Gold} kill:{TotalKills}");

    }
    public IEnumerator GameWaitSecondsResume(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Resume();

    }
}

public enum EGameState
{
    Playing,
    Paused,
    GameOver,
    Victory
}

public enum GameMode
{
    Standard,
    Endless
}