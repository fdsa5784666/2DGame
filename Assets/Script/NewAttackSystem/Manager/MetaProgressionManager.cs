//using UnityEngine;
//using System.Collections.Generic;
//using System.IO;

//public class MetaProgressionManager : MonoBehaviour
//{
//    public static MetaProgressionManager Instance { get; private set; }

//    [Header("=== 永久数据 ===")]
//    [SerializeField] private int totalGold = 0;
//    [SerializeField] private int totalKills = 0;
//    [SerializeField] private int gamesPlayed = 0;
//    [SerializeField] private int gamesWon = 0;

//    [Header("=== 解锁内容 ===")]
//    [SerializeField] private List<CharacterData> unlockedCharacters = new List<CharacterData>();
//    [SerializeField] private List<WeaponData> unlockedWeapons = new List<WeaponData>();
//    [SerializeField] private List<MetaUpgradeData> purchasedUpgrades = new List<MetaUpgradeData>();

//    [Header("=== 最高纪录 ===")]
//    [SerializeField] private int endlessModeBestTime = 0;
//    [SerializeField] private int endlessModeBestLevel = 0;
//    [SerializeField] private int standardModeBestTime = 0;
//    [SerializeField] private int maxKillsInOneRun = 0;
//    [SerializeField] private int maxGoldInOneRun = 0;

//    [Header("=== 所有可解锁内容（用于检查）===")]
//    [SerializeField] private List<CharacterData> allCharacters = new List<CharacterData>();
//    [SerializeField] private List<WeaponData> allWeapons = new List<WeaponData>();
//    [SerializeField] private List<MetaUpgradeData> allMetaUpgrades = new List<MetaUpgradeData>();

//    // 全局永久加成（由购买的Meta升级提供）
//    [Header("=== 永久加成数值 ===")]
//    public float permanentDamageBonus = 0f;
//    public float permanentHealthBonus = 0f;
//    public float permanentSpeedBonus = 0f;
//    public float permanentExpBonus = 0f;
//    public float permanentGoldBonus = 0f;
//    public float permanentCritRateBonus = 0f;
//    public float permanentCritDamageBonus = 0f;
//    public float permanentPickupRangeBonus = 0f;
//    public int extraWeaponSlot = 0;
//    public int startingGold = 0;
//    public int extraRerollCount = 0;
//    public int extraBanishCount = 0;
//    public bool hasReroll = false;
//    public bool hasBanish = false;

//    // 属性
//    public int TotalGold => totalGold;
//    public int TotalKills => totalKills;
//    public int GamesPlayed => gamesPlayed;
//    public int GamesWon => gamesWon;
//    public int EndlessModeBestTime => endlessModeBestTime;
//    public int EndlessModeBestLevel => endlessModeBestLevel;
//    public int StandardModeBestTime => standardModeBestTime;
//    public int MaxKillsInOneRun => maxKillsInOneRun;

//    // 事件
//    public event System.Action OnDataUpdated;
//    public event System.Action<CharacterData> OnCharacterUnlocked;
//    public event System.Action<WeaponData> OnWeaponUnlocked;
//    public event System.Action<MetaUpgradeData> OnUpgradePurchased;

//    private string saveFilePath;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            saveFilePath = Application.persistentDataPath + "/metaprogress.json";
//            LoadData();
//            InitializeDefaultUnlocks();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    #region 初始化
//    private void InitializeDefaultUnlocks()
//    {
//        // 解锁默认角色
//        foreach (var character in allCharacters)
//        {
//            if (character.unlockedByDefault && !unlockedCharacters.Contains(character))
//            {
//                unlockedCharacters.Add(character);
//            }
//        }

//        // 解锁默认武器
//        //foreach (var weapon in allWeapons)
//        //{
//        //    if (weapon.unlockedByDefault && !unlockedWeapons.Contains(weapon))
//        //    {
//        //        unlockedWeapons.Add(weapon);
//        //    }
//        //}
//    }
//    #endregion

//    #region 游戏结算
//    public void OnGameFinished(bool victory, int goldEarned, int kills)
//    {
//        gamesPlayed++;
//        totalKills += kills;
//        totalGold += goldEarned;

//        if (victory)
//        {
//            gamesWon++;
//        }

//        // 更新纪录
//        if (kills > maxKillsInOneRun)
//        {
//            maxKillsInOneRun = kills;
//        }
//        if (goldEarned > maxGoldInOneRun)
//        {
//            maxGoldInOneRun = goldEarned;
//        }

//        // 检查解锁
//        CheckUnlocks();

//        Save();
//        OnDataUpdated?.Invoke();
//    }

//    public void OnStandardModeCompleted(int timeSurvived, int kills, int gold)
//    {
//        if (timeSurvived > standardModeBestTime)
//        {
//            standardModeBestTime = timeSurvived;
//        }
//        OnGameFinished(true, gold, kills);
//    }

//    public void OnEndlessModeFinished(int timeSurvived, int levelReached, int kills, int gold)
//    {
//        if (timeSurvived > endlessModeBestTime)
//        {
//            endlessModeBestTime = timeSurvived;
//        }
//        if (levelReached > endlessModeBestLevel)
//        {
//            endlessModeBestLevel = levelReached;
//        }
//        OnGameFinished(false, gold, kills);
//    }
//    #endregion

//    #region 解锁检查
//    private void CheckUnlocks()
//    {
//        // 检查角色解锁
//        foreach (var character in allCharacters)
//        {
//            if (!unlockedCharacters.Contains(character))
//            {
//                if (CheckUnlockCondition(character))
//                {
//                    UnlockCharacter(character);
//                }
//            }
//        }

//        // 检查武器解锁
//        foreach (var weapon in allWeapons)
//        {
//            if (!unlockedWeapons.Contains(weapon))
//            {
//                //if (CheckUnlockCondition(weapon))
//                //{
//                //    UnlockWeapon(weapon);
//                //}
//            }
//        }
//    }

//    private bool CheckUnlockCondition(CharacterData character)
//    {
//        if (character.unlockCost > 0) return false; // 需要购买的不自动解锁

//        return character.unlockCondition switch
//        {
//            "win_standard" => gamesWon >= 1,
//            "win_standard_3" => gamesWon >= 3,
//            "kill_1000" => totalKills >= 1000,
//            "kill_5000" => totalKills >= 5000,
//            "play_10" => gamesPlayed >= 10,
//            "" => false,
//            _ => false
//        };
//    }

//    //private bool CheckUnlockCondition(WeaponData weapon)
//    //{
//    //    if (weapon.unlockCost > 0) return false;

//    //    return weapon.unlockCondition switch
//    //    {
//    //        "win_standard" => gamesWon >= 1,
//    //        "kill_500" => totalKills >= 500,
//    //        "reach_level_20" => endlessModeBestLevel >= 20,
//    //        "" => false,
//    //        _ => false
//    //    };
//    //}

//    public void UnlockCharacter(CharacterData character)
//    {
//        if (!unlockedCharacters.Contains(character))
//        {
//            unlockedCharacters.Add(character);
//            OnCharacterUnlocked?.Invoke(character);
//            Save();
//        }
//    }

//    public void UnlockWeapon(WeaponData weapon)
//    {
//        if (!unlockedWeapons.Contains(weapon))
//        {
//            unlockedWeapons.Add(weapon);
//            OnWeaponUnlocked?.Invoke(weapon);
//            Save();
//        }
//    }
//    #endregion

//    #region 购买与消费
//    public bool TryPurchaseCharacter(CharacterData character)
//    {
//        if (unlockedCharacters.Contains(character)) return false;
//        if (totalGold < character.unlockCost) return false;

//        totalGold -= character.unlockCost;
//        unlockedCharacters.Add(character);
//        OnCharacterUnlocked?.Invoke(character);
//        Save();
//        OnDataUpdated?.Invoke();
//        return true;
//    }

//    public bool TryPurchaseWeapon(WeaponData weapon)
//    {
//        if (unlockedWeapons.Contains(weapon)) return false;
//        //if (totalGold < weapon.unlockCost) return false;

//        //totalGold -= weapon.unlockCost;
//        unlockedWeapons.Add(weapon);
//        OnWeaponUnlocked?.Invoke(weapon);
//        Save();
//        OnDataUpdated?.Invoke();
//        return true;
//    }

//    public bool TryPurchaseMetaUpgrade(MetaUpgradeData upgrade)
//    {
//        if (!upgrade.CanPurchase()) return false;

//        int cost = upgrade.GetCurrentCost();
//        if (totalGold < cost) return false;

//        totalGold -= cost;
//        upgrade.Purchase();

//        if (!purchasedUpgrades.Contains(upgrade))
//        {
//            purchasedUpgrades.Add(upgrade);
//        }

//        ApplyMetaUpgrade(upgrade);
//        OnUpgradePurchased?.Invoke(upgrade);
//        Save();
//        OnDataUpdated?.Invoke();
//        return true;
//    }

//    public void ApplyMetaUpgrade(MetaUpgradeData upgrade)
//    {
//        switch (upgrade.effectType)
//        {
//            case EMetaUpgradeEffectType.StatBonus:
//                permanentDamageBonus += upgrade.damageBonus;
//                permanentHealthBonus += upgrade.healthBonus;
//                permanentSpeedBonus += upgrade.speedBonus;
//                permanentExpBonus += upgrade.expBonus;
//                permanentGoldBonus += upgrade.goldBonus;
//                permanentCritRateBonus += upgrade.critRateBonus;
//                permanentCritDamageBonus += upgrade.critDamageBonus;
//                permanentPickupRangeBonus += upgrade.pickupRangeBonus;
//                break;

//            case EMetaUpgradeEffectType.ExtraSlot:
//                extraWeaponSlot += upgrade.intValue;
//                break;

//            case EMetaUpgradeEffectType.StartingGold:
//                startingGold += upgrade.intValue;
//                break;

//            case EMetaUpgradeEffectType.RerollUnlock:
//                hasReroll = true;
//                extraRerollCount += upgrade.intValue;
//                break;

//            case EMetaUpgradeEffectType.BanishUnlock:
//                hasBanish = true;
//                extraBanishCount += upgrade.intValue;
//                break;
//        }
//    }

//    public void AddGold(int amount)
//    {
//        totalGold += amount;
//        OnDataUpdated?.Invoke();
//        Save();
//    }
//    #endregion

//    #region 查询方法
//    public bool IsCharacterUnlocked(CharacterData character)
//    {
//        return unlockedCharacters.Contains(character);
//    }

//    public bool IsWeaponUnlocked(WeaponData weapon)
//    {
//        return unlockedWeapons.Contains(weapon);
//    }

//    public bool IsMetaUpgradePurchased(MetaUpgradeData upgrade)
//    {
//        return purchasedUpgrades.Contains(upgrade);
//    }

//    public List<CharacterData> GetUnlockedCharacters()
//    {
//        return new List<CharacterData>(unlockedCharacters);
//    }

//    public List<WeaponData> GetUnlockedWeapons()
//    {
//        return new List<WeaponData>(unlockedWeapons);
//    }

//    public List<MetaUpgradeData> GetPurchasedUpgrades()
//    {
//        return new List<MetaUpgradeData>(purchasedUpgrades);
//    }

//    public List<CharacterData> GetAllCharacters()
//    {
//        return new List<CharacterData>(allCharacters);
//    }

//    public List<WeaponData> GetAllWeapons()
//    {
//        return new List<WeaponData>(allWeapons);
//    }

//    public List<MetaUpgradeData> GetAllMetaUpgrades()
//    {
//        return new List<MetaUpgradeData>(allMetaUpgrades);
//    }
//    #endregion

//    #region 应用永久加成到新游戏
//    public void ApplyPermanentBonusesToGameManager()
//    {
//        var gm = GameData.Instance;
//        if (gm == null) return;

//        gm.AddDamageMultiplier(permanentDamageBonus);
//        gm.AddMaxHealthBonus(permanentHealthBonus);
//        gm.playerCurrentHealth = gm.playerMaxHealth;
//        gm.playerSpeed += permanentSpeedBonus;
//        gm.expMultiplier += permanentExpBonus;
//        gm.goldMultiplier += permanentGoldBonus;
//        gm.AddCriticalRate(permanentCritRateBonus);
//        gm.AddCriticalDamage(permanentCritDamageBonus);
//        gm.pickupRange += permanentPickupRangeBonus;

//        gm.AddGold(startingGold);
//    }

//    public int GetTotalExtraWeaponSlots()
//    {
//        return extraWeaponSlot;
//    }

//    public int GetTotalRerollCount()
//    {
//        return hasReroll ? 1 + extraRerollCount : 0;
//    }

//    public int GetTotalBanishCount()
//    {
//        return hasBanish ? 1 + extraBanishCount : 0;
//    }
//    #endregion

//    #region 存档系统
//    [System.Serializable]
//    private class SaveData
//    {
//        public int totalGold;
//        public int totalKills;
//        public int gamesPlayed;
//        public int gamesWon;
//        public int endlessModeBestTime;
//        public int endlessModeBestLevel;
//        public int standardModeBestTime;
//        public int maxKillsInOneRun;
//        public int maxGoldInOneRun;

//        public List<string> unlockedCharacterNames = new List<string>();
//        public List<string> unlockedWeaponNames = new List<string>();
//        public List<string> purchasedUpgradeNames = new List<string>();
//        public List<int> purchasedUpgradeStacks = new List<int>();

//        // 永久加成
//        public float permanentDamageBonus;
//        public float permanentHealthBonus;
//        public float permanentSpeedBonus;
//        public float permanentExpBonus;
//        public float permanentGoldBonus;
//        public float permanentCritRateBonus;
//        public float permanentCritDamageBonus;
//        public float permanentPickupRangeBonus;
//        public int extraWeaponSlot;
//        public int startingGold;
//        public int extraRerollCount;
//        public int extraBanishCount;
//        public bool hasReroll;
//        public bool hasBanish;
//    }

//    private void Save()
//    {
//        var save = new SaveData
//        {
//            totalGold = totalGold,
//            totalKills = totalKills,
//            gamesPlayed = gamesPlayed,
//            gamesWon = gamesWon,
//            endlessModeBestTime = endlessModeBestTime,
//            endlessModeBestLevel = endlessModeBestLevel,
//            standardModeBestTime = standardModeBestTime,
//            maxKillsInOneRun = maxKillsInOneRun,
//            maxGoldInOneRun = maxGoldInOneRun,

//            permanentDamageBonus = permanentDamageBonus,
//            permanentHealthBonus = permanentHealthBonus,
//            permanentSpeedBonus = permanentSpeedBonus,
//            permanentExpBonus = permanentExpBonus,
//            permanentGoldBonus = permanentGoldBonus,
//            permanentCritRateBonus = permanentCritRateBonus,
//            permanentCritDamageBonus = permanentCritDamageBonus,
//            permanentPickupRangeBonus = permanentPickupRangeBonus,
//            extraWeaponSlot = extraWeaponSlot,
//            startingGold = startingGold,
//            extraRerollCount = extraRerollCount,
//            extraBanishCount = extraBanishCount,
//            hasReroll = hasReroll,
//            hasBanish = hasBanish
//        };

//        // 保存解锁的角色名称
//        foreach (var character in unlockedCharacters)
//        {
//            save.unlockedCharacterNames.Add(character.characterName);
//        }

//        // 保存解锁的武器名称
//        foreach (var weapon in unlockedWeapons)
//        {
//            save.unlockedWeaponNames.Add(weapon.weaponName);
//        }

//        // 保存购买的升级
//        foreach (var upgrade in purchasedUpgrades)
//        {
//            save.purchasedUpgradeNames.Add(upgrade.upgradeName);
//            save.purchasedUpgradeStacks.Add(upgrade.currentStacks);
//        }

//        string json = JsonUtility.ToJson(save, true);
//        File.WriteAllText(saveFilePath, json);

//        Debug.Log($"存档已保存到: {saveFilePath}");
//    }

//    private void LoadData()
//    {
//        if (!File.Exists(saveFilePath))
//        {
//            Debug.Log("未找到存档文件，使用默认数据");
//            return;
//        }

//        string json = File.ReadAllText(saveFilePath);
//        var save = JsonUtility.FromJson<SaveData>(json);

//        totalGold = save.totalGold;
//        totalKills = save.totalKills;
//        gamesPlayed = save.gamesPlayed;
//        gamesWon = save.gamesWon;
//        endlessModeBestTime = save.endlessModeBestTime;
//        endlessModeBestLevel = save.endlessModeBestLevel;
//        standardModeBestTime = save.standardModeBestTime;
//        maxKillsInOneRun = save.maxKillsInOneRun;
//        maxGoldInOneRun = save.maxGoldInOneRun;

//        permanentDamageBonus = save.permanentDamageBonus;
//        permanentHealthBonus = save.permanentHealthBonus;
//        permanentSpeedBonus = save.permanentSpeedBonus;
//        permanentExpBonus = save.permanentExpBonus;
//        permanentGoldBonus = save.permanentGoldBonus;
//        permanentCritRateBonus = save.permanentCritRateBonus;
//        permanentCritDamageBonus = save.permanentCritDamageBonus;
//        permanentPickupRangeBonus = save.permanentPickupRangeBonus;
//        extraWeaponSlot = save.extraWeaponSlot;
//        startingGold = save.startingGold;
//        extraRerollCount = save.extraRerollCount;
//        extraBanishCount = save.extraBanishCount;
//        hasReroll = save.hasReroll;
//        hasBanish = save.hasBanish;

//        // 恢复解锁的角色
//        foreach (string name in save.unlockedCharacterNames)
//        {
//            var character = allCharacters.Find(c => c.characterName == name);
//            if (character != null)
//                unlockedCharacters.Add(character);
//        }

//        // 恢复解锁的武器
//        foreach (string name in save.unlockedWeaponNames)
//        {
//            var weapon = allWeapons.Find(w => w.weaponName == name);
//            if (weapon != null)
//                unlockedWeapons.Add(weapon);
//        }

//        // 恢复购买的升级
//        for (int i = 0; i < save.purchasedUpgradeNames.Count; i++)
//        {
//            var upgrade = allMetaUpgrades.Find(u => u.upgradeName == save.purchasedUpgradeNames[i]);
//            if (upgrade != null)
//            {
//                upgrade.currentStacks = save.purchasedUpgradeStacks[i];
//                upgrade.isPurchased = true;
//                purchasedUpgrades.Add(upgrade);
//            }
//        }

//        Debug.Log($"存档已加载: {saveFilePath}");
//    }

//    [ContextMenu("删除存档")]
//    public void DeleteSaveData()
//    {
//        if (File.Exists(saveFilePath))
//        {
//            File.Delete(saveFilePath);
//            Debug.Log("存档已删除");
//        }
//    }

//    [ContextMenu("打印存档路径")]
//    public void PrintSavePath()
//    {
//        Debug.Log($"存档路径: {saveFilePath}");
//    }
//    #endregion
//}