using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏初始化工具 - 确保所有单例按正确顺序初始化
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("=== 初始化配置 ===")]
    [SerializeField] private bool autoInitialize = true;
    [SerializeField] private GameObject[] managerPrefabs;

    [Header("=== 测试配置 ===")]
    [SerializeField] private bool skipIntro = false;
    [SerializeField] private CharacterData testCharacter;

    void Awake()
    {
        if (autoInitialize)
        {
            InitializeManagers();
        }
    }

    void Start()
    {
        if (testCharacter != null && GameManager.Instance != null)
        {
            StartTestGame();
        }
    }

    private void InitializeManagers()
    {
        // 检查并创建缺失的管理器
        foreach (var prefab in managerPrefabs)
        {
            if (prefab == null) continue;

            var existing = FindAnyObjectByType(prefab.GetType());
            if (existing == null)
            {
                Instantiate(prefab);
                Debug.Log($"自动创建管理器: {prefab.name}");
            }
        }

        // 确保必要的管理器存在
        EnsureManager<GameManager>("GameManager");
        EnsureManager<WeaponSlotManager>("WeaponSlotManager");
        EnsureManager<UpgradeManager>("UpgradeManager");
        EnsureManager<AttackManager>("AttackManager");
        EnsureManager<EnemySpawner>("EnemySpawner");
        EnsureManager<UIManager>("UIManager");
        EnsureManager<MetaProgressionManager>("MetaProgressionManager");
        EnsureManager<VolumeComtroller>("AudioManager");
        EnsureManager<ObjectPool>("ObjectPool");
    }

    private void EnsureManager<T>(string name) where T : MonoBehaviour
    {
        var existing = FindAnyObjectByType<T>();
        if (existing == null)
        {
            var go = new GameObject(name);
            go.AddComponent<T>();
            Debug.Log($"创建缺失的管理器: {name}");
        }
    }

    private void StartTestGame()
    {
        var gm = GameManager.Instance;
        var slotManager = WeaponSlotManager.Instance;

        if (gm != null && slotManager != null)
        {
            // 选择测试角色
            //if (testCharacter != null)
            //{
            //    gm.SelectCharacter(testCharacter);
            //}

            // 跳过开场（如果需要）
            if (skipIntro)
            {
                // 直接开始游戏逻辑
            }
        }
    }

    /// <summary>
    /// 重启游戏
    /// </summary>
    public static void RestartGame()
    {
        // 重置所有升级的选择次数
        var upgradeManager = UpgradeManager.Instance;
        if (upgradeManager != null)
        {
            upgradeManager.ResetForNewGame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public static void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}