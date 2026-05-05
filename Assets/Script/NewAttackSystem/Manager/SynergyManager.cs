using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynergyManager : MonoBehaviour
{
    public static SynergyManager Instance { get; private set; }
    [Header("配置")]
    [SerializeField] private List<SynergyConfig> synergies;  // 所有羁绊配置

    [Header("依赖引用")]
    [SerializeField] private GameData gameData;
    [SerializeField] private WeaponSlotManager weaponManager;

    // 当前装备的武器标签统计
    private Dictionary<EWeaponTag, int> currentTagCount = new Dictionary<EWeaponTag, int>();

    public event Action<SynergyConfig,int> OnSynergyUpdated;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
        weaponManager.OnWeaponAdded += ( data, level) => Refresh();
        weaponManager.OnWeaponRemoved += (slotIndex) => Refresh();
        Refresh();
    }

    /// <summary>
    /// 刷新所有羁绊（武器变化时调用）
    /// </summary>
    public void Refresh()
    {
        //统计标签数量
        //currentTagCount = new Dictionary<EWeaponTag, int>();
        //foreach(var weapon in weaponManager.EquippedWeapons)
        //{
        //    if (weapon?.tags == null) continue;
        //    foreach(var tag in weapon.tags)
        //    {
        //        currentTagCount[tag] = currentTagCount.GetValueOrDefault(tag) + 1;
        //    }
        //}
        currentTagCount = GetDistincTagCount();

        //重置羁绊加成
        gameData.ResetSynergyBonuses();
        //应用羁绊
        foreach (var synergy in synergies)
        {
            int count = currentTagCount.GetValueOrDefault(synergy.requiredTag, 0);
            int tier = GetTier(synergy, count);
            if (tier >= 0)
            {
                synergy.buffs[tier].Apply(gameData);
                OnSynergyUpdated?.Invoke(synergy,tier);
            }
        }

    }
    int GetTier(SynergyConfig synergy,int count)
    {
        for(int i = synergy.thresholds.Length - 1;i>=0;i--)
        {
            if (count >= synergy.thresholds[i]) return i;
        }
        return -1;
    }
    private Dictionary<EWeaponTag,int > GetDistincTagCount()
    {
        var tagCount = new Dictionary<EWeaponTag, int>();
        var countedWeapontypes = new HashSet<WeaponData>();

        foreach (var weapon in weaponManager.EquippedWeapons)
        {
            if (weapon == null) continue;

            if(countedWeapontypes.Contains(weapon)) continue; // 同类型武器只统计一次
            countedWeapontypes.Add(weapon);

            foreach (var tag in weapon.tags)
            {
                tagCount[tag] = tagCount.GetValueOrDefault(tag) + 1;
            }

        }
        return tagCount;
    }

}
[System.Serializable]
public class SynergyConfig
{
    public string name;
    public EWeaponTag requiredTag;
    public int[] thresholds = { 2, 4, 6 };
    public SynergyBuff[] buffs;  // 长度 = thresholds.Length
}

[System.Serializable]
public class SynergyBuff
{
    public float damageBonus;
    public float attackSpeedBonus;
    public float critRateBonus;

    public void Apply(GameData gameData, float multiplier = 1f)
    {
        gameData.AddDamageMultiplier(damageBonus);
        gameData.AddAttackSpeedBonus(attackSpeedBonus);
        gameData.AddCriticalRate(critRateBonus);
    }
}