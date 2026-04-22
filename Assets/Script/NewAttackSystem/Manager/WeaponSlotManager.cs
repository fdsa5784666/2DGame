using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WeaponSlotManager : MonoBehaviour
{
    public static WeaponSlotManager Instance { get; private set; }

    [Header("=== 槽位配置 ===")]
    [SerializeField] private int maxSlotCount = 6;
    [SerializeField] private List<WeaponInstance> equippedWeapons = new List<WeaponInstance>();

    [Header("=== 初始武器（角色绑定）===")]
    [SerializeField] private WeaponData lockedStartingWeapon;

    [Header("=== 标签计数 ===")]
    private Dictionary<EWeaponTag, int> weaponTagCounts = new Dictionary<EWeaponTag, int>();
    private Dictionary<EWeaponTag, int> virtualTagCounts = new Dictionary<EWeaponTag, int>();

    [Header("=== 羁绊配置 ===")]
    [SerializeField] private List<SynergyData> allSynergies = new List<SynergyData>();
    private List<ActiveSynergy> activeSynergies = new List<ActiveSynergy>();

    // 事件
    public event System.Action<WeaponInstance> OnWeaponAdded;
    public event System.Action<WeaponInstance, int> OnWeaponUpgraded;
    public event System.Action<WeaponInstance> OnWeaponRemoved;
    public event System.Action<SynergyData, int> OnSynergyActivated;
    public event System.Action<SynergyData, int> OnSynergyDeactivated;
    public event System.Action OnSynergiesUpdated;

    // 属性
    public int MaxSlotCount => maxSlotCount;
    public int UsedSlotCount => equippedWeapons.Count;
    public int EmptySlotCount => maxSlotCount - equippedWeapons.Count;
    public bool HasEmptySlot => equippedWeapons.Count < maxSlotCount;
    public WeaponData LockedStartingWeapon => lockedStartingWeapon;
    public IReadOnlyList<WeaponInstance> EquippedWeapons => equippedWeapons;
    public IReadOnlyList<ActiveSynergy> ActiveSynergies => activeSynergies;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 初始化标签字典
        foreach (EWeaponTag tag in System.Enum.GetValues(typeof(EWeaponTag)))
        {
            weaponTagCounts[tag] = 0;
            virtualTagCounts[tag] = 0;
        }
    }

    #region 武器管理
    /// <summary>
    /// 尝试添加武器
    /// </summary>
    public bool TryAddWeapon(WeaponData data)
    {
        if (data == null) return false;

        // 检查是否已有同款武器
        WeaponInstance existing = equippedWeapons.Find(w => w.Data == data);
        if (existing != null && existing.CanLevelUp)
        {
            existing.LevelUp();
            OnWeaponUpgraded?.Invoke(existing, existing.Level);
            RecalculateAllTags();
            return true;
        }

        // 检查是否有空槽位
        if (!HasEmptySlot) return false;

        // 创建新实例
        WeaponInstance newWeapon = new WeaponInstance(data);

        // 如果是第一个武器且没有锁定武器，则锁定它
        if (equippedWeapons.Count == 0 && lockedStartingWeapon == null)
        {
            lockedStartingWeapon = data;
        }

        equippedWeapons.Add(newWeapon);
        OnWeaponAdded?.Invoke(newWeapon);
        RecalculateAllTags();

        return true;
    }

    /// <summary>
    /// 移除武器
    /// </summary>
    public bool RemoveWeapon(WeaponInstance weapon)
    {
        // 不能移除锁定武器
        if (weapon.Data == lockedStartingWeapon) return false;

        if (equippedWeapons.Remove(weapon))
        {
            OnWeaponRemoved?.Invoke(weapon);
            RecalculateAllTags();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 替换武器（用于置换系统）
    /// </summary>
    public bool ReplaceWeapon(WeaponInstance oldWeapon, WeaponData newData)
    {
        if (oldWeapon.Data == lockedStartingWeapon) return false;
        if (newData == null) return false;

        int index = equippedWeapons.IndexOf(oldWeapon);
        if (index == -1) return false;

        // 检查新武器是否已存在（可叠加）
        WeaponInstance existing = equippedWeapons.Find(w => w.Data == newData);
        if (existing != null && existing.CanLevelUp)
        {
            // 移除旧武器，升级新武器
            equippedWeapons.RemoveAt(index);
            existing.LevelUp();
            OnWeaponRemoved?.Invoke(oldWeapon);
            OnWeaponUpgraded?.Invoke(existing, existing.Level);
        }
        else
        {
            // 创建新武器，继承部分等级
            WeaponInstance newWeapon = new WeaponInstance(newData);
            newWeapon.InheritFrom(oldWeapon); // 继承50%的加成
            equippedWeapons[index] = newWeapon;
            OnWeaponRemoved?.Invoke(oldWeapon);
            OnWeaponAdded?.Invoke(newWeapon);
        }

        RecalculateAllTags();
        return true;
    }

    /// <summary>
    /// 初始化角色起始武器
    /// </summary>
    public void InitializeStartingWeapon(WeaponData weapon)
    {
        lockedStartingWeapon = weapon;
        TryAddWeapon(weapon);
    }

    /// <summary>
    /// 根据名称获取武器
    /// </summary>
    public WeaponInstance GetWeaponByName(string name)
    {
        return equippedWeapons.Find(w => w.Data.weaponName == name);
    }

    /// <summary>
    /// 获取所有武器
    /// </summary>
    public List<WeaponInstance> GetAllWeapons()
    {
        return new List<WeaponInstance>(equippedWeapons);
    }
    #endregion

    #region 标签与羁绊
    /// <summary>
    /// 添加虚拟标签（碎片系统）
    /// </summary>
    public void AddVirtualTag(EWeaponTag tag)
    {
        virtualTagCounts[tag]++;
        RecalculateAllTags();
    }

    /// <summary>
    /// 获取标签总计数（武器 + 虚拟）
    /// </summary>
    public int GetTagCount(EWeaponTag tag)
    {
        return weaponTagCounts.GetValueOrDefault(tag, 0) + virtualTagCounts.GetValueOrDefault(tag, 0);
    }

    /// <summary>
    /// 获取所有标签计数
    /// </summary>
    public Dictionary<EWeaponTag, int> GetAllTagCounts()
    {
        var result = new Dictionary<EWeaponTag, int>();
        foreach (EWeaponTag tag in System.Enum.GetValues(typeof(EWeaponTag)))
        {
            result[tag] = GetTagCount(tag);
        }
        return result;
    }

    /// <summary>
    /// 重新计算所有标签
    /// </summary>
    private void RecalculateAllTags()
    {
        // 重置武器标签计数
        foreach (EWeaponTag tag in System.Enum.GetValues(typeof(EWeaponTag)))
        {
            weaponTagCounts[tag] = 0;
        }

        // 重新计算（高品质武器贡献更多计数）
        foreach (var weapon in equippedWeapons)
        {
            foreach (var tag in weapon.Data.tags)
            {
                weaponTagCounts[tag] += weapon.Level;
            }
        }

        CheckSynergies();
        OnSynergiesUpdated?.Invoke();
    }

    /// <summary>
    /// 检查羁绊激活状态
    /// </summary>
    private void CheckSynergies()
    {
        var newActiveSynergies = new List<ActiveSynergy>();

        foreach (var synergy in allSynergies)
        {
            int count = GetTagCount(synergy.requiredTag);
            int tier = synergy.GetActivatedTier(count);

            if (tier >= 0)
            {
                newActiveSynergies.Add(new ActiveSynergy
                {
                    data = synergy,
                    tier = tier,
                    count = count
                });
            }
        }

        // 检查变化
        foreach (var newActive in newActiveSynergies)
        {
            var oldActive = activeSynergies.Find(a => a.data == newActive.data);
            if (oldActive == null)
            {
                // 新激活
                newActive.data.ApplyEffect(newActive.tier, GameManager.Instance, this);
                OnSynergyActivated?.Invoke(newActive.data, newActive.tier);
            }
            else if (oldActive.tier != newActive.tier)
            {
                // 等级变化
                newActive.data.ApplyEffect(newActive.tier, GameManager.Instance, this);
                OnSynergyActivated?.Invoke(newActive.data, newActive.tier);
            }
        }

        // 检查失效
        foreach (var oldActive in activeSynergies)
        {
            var newActive = newActiveSynergies.Find(a => a.data == oldActive.data);
            if (newActive == null)
            {
                OnSynergyDeactivated?.Invoke(oldActive.data, oldActive.tier);
            }
        }

        activeSynergies = newActiveSynergies;
    }

    /// <summary>
    /// 检查是否已激活某羁绊
    /// </summary>
    public bool IsSynergyActive(SynergyData synergy, out int tier)
    {
        var active = activeSynergies.Find(a => a.data == synergy);
        if (active != null)
        {
            tier = active.tier;
            return true;
        }
        tier = -1;
        return false;
    }

    /// <summary>
    /// 获取距离下一羁绊等级还差多少
    /// </summary>
    public int GetMissingForNextTier(SynergyData synergy)
    {
        int currentCount = GetTagCount(synergy.requiredTag);
        return synergy.GetMissingCount(currentCount);
    }
    #endregion

    #region 快照与存档
    public List<WeaponSnapshot> GetWeaponSnapshots()
    {
        return equippedWeapons.Select(w => w.CreateSnapshot()).ToList();
    }

    public List<string> GetActiveSynergyIds()
    {
        return activeSynergies.Select(a => a.data.synergyName).ToList();
    }

    public void LoadFromSnapshot(BuildSnapshot snapshot)
    {
        equippedWeapons.Clear();
        foreach (var ws in snapshot.weapons)
        {
            // 需要根据名称加载 WeaponData
            // 这里简化处理，实际需要资源加载系统
        }
        RecalculateAllTags();
    }
    #endregion
}

#region 辅助类
[System.Serializable]
public class WeaponInstance
{
    [SerializeField] private WeaponData data;
    [SerializeField] private int level = 1;

    // 来自升级的额外加成
    public float damageBonus = 0f;
    public float attackSpeedBonus = 0f;
    public float rangeBonus = 0f;
    public int projectileBonus = 0;
    public float bulletSpeedBonus = 0f;
    public float bulletSizeBonus = 0f;
    public int pierceBonus = 0;

    public WeaponData Data => data;
    public int Level => level;
    public bool CanLevelUp => level < data.maxLevel;

    // 计算属性
    public float FinalDamage => data.baseDamage * data.GetDamageMultiplier(level) * (1 + damageBonus);
    public float FinalAttackSpeed => data.baseAttackSpeed * data.GetAttackSpeedMultiplier(level) * (1 + attackSpeedBonus);
    public int FinalProjectileCount => Mathf.RoundToInt(data.baseProjectileCount * data.GetProjectileMultiplier(level)) + projectileBonus;
    public float FinalRange => data.baseRange + rangeBonus;
    public int FinalPierceCount => (data.bulletData?.pierceCount ?? 1) + pierceBonus;
    public float FinalBulletSpeed => (data.bulletData?.speed ?? 8f) * (1 + bulletSpeedBonus);
    public float FinalBulletSize => (data.bulletData?.size ?? 1f) * (1 + bulletSizeBonus);

    public WeaponInstance(WeaponData weaponData)
    {
        data = weaponData;
        level = 1;
    }

    public void LevelUp()
    {
        if (CanLevelUp) level++;
    }

    public void ApplyBonus(UpgradeData upgrade)
    {
        damageBonus += upgrade.damageBonus;
        attackSpeedBonus += upgrade.attackSpeedBonus;
        projectileBonus += upgrade.projectileBonus;
        pierceBonus += upgrade.pierceBonus;
        bulletSpeedBonus += upgrade.bulletSpeedBonus;
        bulletSizeBonus += upgrade.bulletSizeBonus;
        rangeBonus += upgrade.rangeBonus;
    }

    /// <summary>
    /// 从另一把武器继承部分加成（用于置换）
    /// </summary>
    public void InheritFrom(WeaponInstance other)
    {
        level = Mathf.Max(1, other.level - 1);
        damageBonus = other.damageBonus * 0.5f;
        attackSpeedBonus = other.attackSpeedBonus * 0.5f;
        projectileBonus = Mathf.FloorToInt(other.projectileBonus * 0.5f);
        pierceBonus = Mathf.FloorToInt(other.pierceBonus * 0.5f);
        bulletSpeedBonus = other.bulletSpeedBonus * 0.5f;
        bulletSizeBonus = other.bulletSizeBonus * 0.5f;
        rangeBonus = other.rangeBonus * 0.5f;
    }

    public WeaponSnapshot CreateSnapshot()
    {
        return new WeaponSnapshot
        {
            weaponName = data.weaponName,
            level = level,
            damageBonus = damageBonus,
            attackSpeedBonus = attackSpeedBonus,
            projectileBonus = projectileBonus,
            pierceBonus = pierceBonus,
            bulletSpeedBonus = bulletSpeedBonus,
            bulletSizeBonus = bulletSizeBonus,
            rangeBonus = rangeBonus
        };
    }
}

[System.Serializable]
public class ActiveSynergy
{
    public SynergyData data;
    public int tier;
    public int count;
}
#endregion