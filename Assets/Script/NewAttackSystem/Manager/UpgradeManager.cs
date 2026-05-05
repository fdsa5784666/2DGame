using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("=== 升级池 ===")]
    [SerializeField] private List<UpgradeData> allUpgrades = new List<UpgradeData>();
    [SerializeField] private int optionsPerLevel = 3;

    [Header("=== 碎片配置 ===")]
    [SerializeField] private List<SynergyFragmentData> fragmentUpgrades = new List<SynergyFragmentData>();

    [Header("=== 置换配置 ===")]
    [SerializeField] private float guaranteedReplaceTime = 480f; // 8分钟
    private bool replaceUsed = false;

    [Header("=== UI引用 ===")]
    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private Transform upgradePanelParent;
    [SerializeField] private GameObject replacePanelPrefab;

    private GameObject currentPanel;
    private bool isWaitingForChoice = false;

    // 事件
    public event System.Action<List<UpgradeData>> OnUpgradeOptionsShown;
    public event System.Action<UpgradeData> OnUpgradeSelected;
    public event System.Action OnReplaceShown;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 订阅游戏时间事件，用于触发强制置换
        if (GameData.Instance != null)
        {
            GameData.Instance.OnTimerUpdated += CheckGuaranteedReplace;
        }
    }

    void OnDestroy()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.OnTimerUpdated -= CheckGuaranteedReplace;
        }
    }

    #region 升级选项
    /// <summary>
    /// 显示升级选项
    /// </summary>
    public void ShowUpgradeOptions()
    {
        if (isWaitingForChoice) return;

        List<UpgradeData> available = GetAvailableUpgrades();
        List<UpgradeData> selected = PickByWeight(available, optionsPerLevel);

        if (selected.Count == 0)
        {
            // 没有可用升级，直接恢复游戏
            GameData.Instance.Resume();
            return;
        }

        isWaitingForChoice = true;
        OnUpgradeOptionsShown?.Invoke(selected);

        // 创建UI
        if (upgradePanelPrefab != null)
        {
            currentPanel = Instantiate(upgradePanelPrefab, upgradePanelParent ?? transform);
            //var upgradeUI = currentPanel.GetComponent<UpgradeUI>();
            //if (upgradeUI != null)
            //{
            //    upgradeUI.Show(selected, OnUpgradeChosen);
            //}
        }
    }

    /// <summary>
    /// 获取当前可用的升级选项
    /// </summary>
    private List<UpgradeData> GetAvailableUpgrades()
    {
        var available = new List<UpgradeData>();
        int playerLevel = GameData.Instance?.playerLevel ?? 1;
        var slotManager = WeaponSlotManager.Instance;
        var gameData = GameData.Instance;

        // 添加普通升级
        foreach (var upgrade in allUpgrades)
        {
            if (upgrade.CanAppear(playerLevel, slotManager, gameData))
            {
                available.Add(upgrade);
            }
        }

        // 添加碎片升级（动态生成）
        foreach (var fragment in fragmentUpgrades)
        {
            if (fragment.CanAppear(playerLevel, slotManager, gameData))
            {
                available.Add(fragment);
            }
        }

        return available;
    }

    /// <summary>
    /// 按权重抽取
    /// </summary>
    private List<UpgradeData> PickByWeight(List<UpgradeData> options, int count)
    {
        if (options.Count <= count)
        {
            return ShuffleList(options);
        }

        var selected = new List<UpgradeData>();
        var pool = new List<UpgradeData>(options);
        var slotManager = WeaponSlotManager.Instance;

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            // 计算动态权重
            var weights = pool.Select(u => CalculateDynamicWeight(u, slotManager)).ToList();
            float totalWeight = weights.Sum();

            if (totalWeight <= 0)
            {
                // 权重全为0，随机选
                int randomIndex = Random.Range(0, pool.Count);
                selected.Add(pool[randomIndex]);
                pool.RemoveAt(randomIndex);
                continue;
            }

            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            int pickedIndex = 0;

            for (int j = 0; j < pool.Count; j++)
            {
                cumulative += weights[j];
                if (randomValue <= cumulative)
                {
                    pickedIndex = j;
                    break;
                }
            }

            selected.Add(pool[pickedIndex]);
            pool.RemoveAt(pickedIndex);
        }

        return selected;
    }

    /// <summary>
    /// 计算动态权重（智能适配Build）
    /// </summary>
    private float CalculateDynamicWeight(UpgradeData upgrade, WeaponSlotManager slotManager)
    {
        float weight = upgrade.GetBaseWeight();

        //如果是碎片，检查是否差一点激活羁绊
        if (upgrade is SynergyFragmentData fragment)
        {
            var synergy = FindSynergyForTag(fragment.fragmentTag);
            if (synergy != null)
            {
                //int missing = slotManager.GetMissingForNextTier(synergy);
                //if (missing == 1)
                //{
                //    weight *= 3f; // 差1个，大幅提高概率
                //}
                //else if (missing == 2)
                //{
                //    weight *= 1.5f;
                //}
            }
        }

        //如果是指定武器升级，且该武器等级较低
        if (!string.IsNullOrEmpty(upgrade.requiredWeaponName))
        {
            var weapon = slotManager.GetWeaponByName(upgrade.requiredWeaponName);
            if (weapon != null && weapon.StarLevel <= 2)
            {
                weight *= 1.5f; // 低等级武器升级优先
            }
        }

        // 如果需要空槽位且有空槽位，提高新武器出现概率
        if (upgrade.requireEmptySlot && slotManager.HasEmptySlot)
        {
            weight *= 1.2f;
        }

        return weight;
    }

    private SynergyData FindSynergyForTag(EWeaponTag tag)
    {
        var slotManager = WeaponSlotManager.Instance;
        // 通过 slotManager 或全局配置查找
        return null; // 简化，实际需要引用羁绊列表
    }

    private List<UpgradeData> ShuffleList(List<UpgradeData> list)
    {
        var shuffled = new List<UpgradeData>(list);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            var temp = shuffled[i];
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }
        return shuffled;
    }

    /// <summary>
    /// 升级被选择
    /// </summary>
    private void OnUpgradeChosen(UpgradeData chosen)
    {
        isWaitingForChoice = false;

        if (chosen != null)
        {
            chosen.Apply(WeaponSlotManager.Instance, GameData.Instance);
            OnUpgradeSelected?.Invoke(chosen);
        }

        if (currentPanel != null)
        {
            Destroy(currentPanel);
            currentPanel = null;
        }

        GameData.Instance.Resume();
        GameData.Instance.FullHP(); // 升级回满血
    }
    #endregion

    #region 置换系统
    private void CheckGuaranteedReplace(float gameTime)
    {
        if (!replaceUsed && gameTime >= guaranteedReplaceTime)
        {
            ShowReplacePanel();
        }
    }

    /// <summary>
    /// 显示置换面板
    /// </summary>
    public void ShowReplacePanel()
    {
        if (replaceUsed) return;
        if (GameData.Instance.CurrentState != EGameState.Playing) return;

        replaceUsed = true;
        GameData.Instance.Pause();

        OnReplaceShown?.Invoke();

        if (replacePanelPrefab != null)
        {
            currentPanel = Instantiate(replacePanelPrefab, upgradePanelParent ?? transform);
            //var replaceUI = currentPanel.GetComponent<ReplacePanel>();
            //if (replaceUI != null)
            //{
            //    replaceUI.Show(OnReplaceConfirmed, OnReplaceCancelled);
            //}
        }
    }

    private void OnReplaceConfirmed(WeaponInstance oldWeapon, WeaponData newWeapon)
    {
        //int index= WeaponSlotManager.Instance.GetWeaponIndex(oldWeapon);
        //WeaponSlotManager.Instance.ReplaceWeapon(index, newWeapon);
        //CloseReplacePanel();
    }

    private void OnReplaceCancelled()
    {
        CloseReplacePanel();
    }

    private void CloseReplacePanel()
    {
        if (currentPanel != null)
        {
            Destroy(currentPanel);
            currentPanel = null;
        }
        GameData.Instance.Resume();
    }
    #endregion

    #region 重置（新游戏时调用）
    public void ResetForNewGame()
    {
        replaceUsed = false;
        isWaitingForChoice = false;

        foreach (var upgrade in allUpgrades)
        {
            upgrade.ResetSelectionCount();
        }
        foreach (var fragment in fragmentUpgrades)
        {
            fragment.ResetSelectionCount();
        }
    }
    #endregion
}

// 碎片升级数据（继承 UpgradeData）
[CreateAssetMenu(fileName = "Fragment_", menuName = "Game/Synergy Fragment")]
public class SynergyFragmentData : UpgradeData
{
    [Header("=== 碎片专属 ===")]
    public EWeaponTag fragmentTag = EWeaponTag.Fire;

    public override bool CanAppear(int playerLevel, WeaponSlotManager slotManager, GameData gameManager)
    {
        // 碎片总是可以出现（只要有这个标签的羁绊）
        return true; // 简化
    }
}