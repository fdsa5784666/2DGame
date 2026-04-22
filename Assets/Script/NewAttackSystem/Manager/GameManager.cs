using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("=== 游戏状态 ===")]
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private float gameTimer = 0f;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToNextLevel = 100;

    [Header("=== 玩家属性 ===")]
    public float playerMaxHealth = 100f;
    public float playerCurrentHealth = 100f;
    public float playerMoveSpeed = 5f;
    public float pickupRange = 3f;

    [Header("=== 全局战斗属性 ===")]
    public float globalDamageMultiplier = 1f;
    public float globalAttackSpeedBonus = 0f;
    public float globalCritRate = 0.05f;
    public float globalCritDamage = 1.5f;

    [Header("=== 资源 ===")]
    [SerializeField] private int gold = 0;
    [SerializeField] private int totalKills = 0;
    public float expMultiplier = 1f;
    public float goldMultiplier = 1f;

    [Header("=== 模式配置 ===")]
    [SerializeField] private GameMode currentMode = GameMode.Standard;
    [SerializeField] private float standardModeDuration = 1200f; // 20分钟 = 1200秒

    [Header("=== 引用 ===")]
    public GameObject player;
    public WeaponSlotManager slotManager;
    public EnemySpawner enemySpawner;
    public UpgradeManager upgradeManager;
    public UIManager uiManager;

    // 事件
    public event Action<float> OnTimerUpdated;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnKillsChanged;
    public event Action<float, float> OnHealthChanged;
    public event Action<int, int, int> OnExpChanged;
    public event Action<GameState> OnStateChanged;
    public event Action<int> OnLevelUp;

    // 属性
    public GameState CurrentState => currentState;
    public float GameTimer => gameTimer;
    public int CurrentLevel => currentLevel;
    public int Gold => gold;
    public int TotalKills => totalKills;
    public GameMode CurrentMode => currentMode;
    public float StandardModeDuration => standardModeDuration;
    public float RemainingTime => Mathf.Max(0, standardModeDuration - gameTimer);

    void Awake()
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

    void InitializeGame()
    {
        currentState = GameState.Playing;
        gameTimer = 0f;
        currentLevel = 1;
        currentExp = 0;
        expToNextLevel = CalculateExpRequired(1);
        playerCurrentHealth = playerMaxHealth;
        gold = 0;
        totalKills = 0;
    }

    void Update()
    {
        if (currentState != GameState.Playing) return;

        // 游戏计时
        gameTimer += Time.deltaTime;
        OnTimerUpdated?.Invoke(gameTimer);

        // 标准模式：检查是否到达20分钟
        if (currentMode == GameMode.Standard && gameTimer >= standardModeDuration)
        {
            Victory();
        }
    }

    #region 经验与升级
    public void AddExp(int amount)
    {
        int actualAmount = Mathf.RoundToInt(amount * expMultiplier);
        currentExp += actualAmount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        OnExpChanged?.Invoke(currentExp, expToNextLevel, currentLevel);
    }

    void LevelUp()
    {
        currentLevel++;
        expToNextLevel = CalculateExpRequired(currentLevel);

        OnLevelUp?.Invoke(currentLevel);
        OnExpChanged?.Invoke(currentExp, expToNextLevel, currentLevel);

        // 暂停游戏并显示升级选项
        Pause();
        upgradeManager?.ShowUpgradeOptions();
    }

    int CalculateExpRequired(int level)
    {
        // 经验公式：100 * 1.2^(level-1)
        return Mathf.RoundToInt(100 * Mathf.Pow(1.2f, level - 1));
    }
    #endregion

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

    public void AddKill()
    {
        totalKills++;
        OnKillsChanged?.Invoke(totalKills);
    }
    #endregion

    #region 生命值
    public void TakeDamage(float damage)
    {
        playerCurrentHealth = Mathf.Max(0, playerCurrentHealth - damage);
        OnHealthChanged?.Invoke(playerCurrentHealth, playerMaxHealth);

        if (playerCurrentHealth <= 0)
        {
            GameOver(false);
        }
    }

    public void Heal(float amount)
    {
        playerCurrentHealth = Mathf.Min(playerMaxHealth, playerCurrentHealth + amount);
        OnHealthChanged?.Invoke(playerCurrentHealth, playerMaxHealth);
    }

    public void FullHeal()
    {
        playerCurrentHealth = playerMaxHealth;
        OnHealthChanged?.Invoke(playerCurrentHealth, playerMaxHealth);
    }
    #endregion

    #region 状态控制
    public void Pause()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        OnStateChanged?.Invoke(currentState);
    }

    public void Resume()
    {
        if (currentState != GameState.Paused) return;
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        OnStateChanged?.Invoke(currentState);
    }

    /// <summary>
    /// 传值victory来区分是胜利还是失败 统一处理游戏结束逻辑
    /// </summary>
    /// <param name="victory"></param>
    public void GameOver(bool victory = false)
    {
        currentState = victory ? GameState.Victory : GameState.GameOver;
        Time.timeScale = 0f;
        OnStateChanged?.Invoke(currentState);

        // 通知元进度管理器
        MetaProgressionManager.Instance?.OnGameFinished(victory, gold, totalKills);

        //推测也要有胜利失败两种情况
        uiManager?.ShowGameOver(victory);
    }

    public void Victory()
    {
        GameOver(true);
    }
    #endregion

    #region 特殊效果
    public void ApplySpecialEffect(string effectId, string parameters)
    {
        // 根据 effectId 应用特殊效果
        Debug.Log($"应用特殊效果: {effectId}, 参数: {parameters}");

        // 这里可以解析 parameters 并应用到对应系统
        // 例如："子弹反弹"、"穿透+1"等
    }
    #endregion

    //#region 无尽模式
    //public void StartEndlessMode(BuildSnapshot snapshot)
    //{
    //    currentMode = GameMode.Endless;
    //    LoadBuildSnapshot(snapshot);
    //    // 切换到无尽模式配置
    //    enemySpawner?.SetEndlessMode(true);
    //}

    //void LoadBuildSnapshot(BuildSnapshot snapshot)
    //{
    //    currentLevel = snapshot.level;
    //    gold = snapshot.gold;
    //    // 加载武器等...
    //    slotManager?.LoadFromSnapshot(snapshot);
    //}

    //public BuildSnapshot CreateBuildSnapshot()
    //{
    //    return new BuildSnapshot
    //    {
    //        level = currentLevel,
    //        gold = gold,
    //        weapons = slotManager?.GetWeaponSnapshots(),
    //        activeSynergies = slotManager?.GetActiveSynergyIds()
    //    };
    //}
    //#endregion
}

#region 枚举与结构体
public enum GameState
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

[System.Serializable]
public class BuildSnapshot
{
    public int level;
    public int gold;
    public System.Collections.Generic.List<WeaponSnapshot> weapons;
    public System.Collections.Generic.List<string> activeSynergies;
}

[System.Serializable]
public class WeaponSnapshot
{
    public string weaponName;
    public int level;
    public float damageBonus;
    public float attackSpeedBonus;
    public int projectileBonus;
    public int pierceBonus;
    public float bulletSpeedBonus;
    public float bulletSizeBonus;
    public float rangeBonus;
}
#endregion