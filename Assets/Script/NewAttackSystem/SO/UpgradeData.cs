using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_", menuName = "Game/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("=== 显示信息 ===")]
    public string upgradeName = "新升级";
    [TextArea(2, 4)]
    public string description = "";
    public Sprite icon;
    public Rarity rarity = Rarity.Common;

    [Header("=== 出现条件 ===")]
    [Tooltip("玩家最低等级")]
    public int minPlayerLevel = 0;
    [Tooltip("玩家最高等级（超过后不再出现）")]
    public int maxPlayerLevel = 999;
    [Tooltip("本升级最多可选择次数")]
    public int maxSelectionCount = 999;
    [Tooltip("需要玩家拥有特定武器（为空则不限制）")]
    public string requiredWeaponName = "";
    [Tooltip("需要玩家拥有特定标签")]
    public List<EWeaponTag> requiredTags = new List<EWeaponTag>();
    [Tooltip("需要有空武器槽位")]
    public bool requireEmptySlot = false;

    [Header("=== 效果类型 ===")]
    public UpgradeEffectType effectType = UpgradeEffectType.StatBonus;

    [Header("=== 数值加成（StatBonus类型使用）===")]
    [Tooltip("伤害百分比加成")]
    public float damageBonus = 0f;
    [Tooltip("攻速百分比加成")]
    public float attackSpeedBonus = 0f;
    [Tooltip("暴击率加成")]
    public float critRateBonus = 0f;
    [Tooltip("暴击伤害加成")]
    public float critDamageBonus = 0f;
    [Tooltip("弹幕数加成")]
    public int projectileBonus = 0;
    [Tooltip("穿透次数加成")]
    public int pierceBonus = 0;
    [Tooltip("子弹速度百分比加成")]
    public float bulletSpeedBonus = 0f;
    [Tooltip("子弹大小百分比加成")]
    public float bulletSizeBonus = 0f;
    [Tooltip("攻击范围加成")]
    public float rangeBonus = 0f;
    [Tooltip("最大生命值加成")]
    public float maxHealthBonus = 0f;
    [Tooltip("移动速度加成")]
    public float moveSpeedBonus = 0f;

    [Header("=== 全局加成（GlobalBonus类型使用）===")]
    [Tooltip("全局伤害倍率")]
    public float globalDamageBonus = 0f;
    [Tooltip("经验获取倍率")]
    public float expMultiplierBonus = 0f;
    [Tooltip("金币获取倍率")]
    public float goldMultiplierBonus = 0f;
    [Tooltip("拾取范围加成")]
    public float pickupRangeBonus = 0f;

    [Header("=== 特殊效果 ===")]
    [Tooltip("新武器（NewWeapon类型使用）")]
    public WeaponData newWeapon;
    [Tooltip("碎片标签（Fragment类型使用）")]
    public EWeaponTag fragmentTag;
    [Tooltip("特殊效果ID（MechanicChange类型使用）")]
    public string specialEffectId = "";
    [Tooltip("特殊效果参数（JSON格式）")]
    public string specialEffectParams = "";

    [System.NonSerialized]
    public int currentSelectionCount = 0; // 当前被选择的次数

    /// <summary>
    /// 检查是否满足出现条件
    /// </summary>
    public virtual bool CanAppear(int playerLevel, WeaponSlotManager slotManager, GameData gameData)
    {
        // 等级条件
        if (playerLevel < minPlayerLevel || playerLevel > maxPlayerLevel)
            return false;

        // 选择次数条件
        if (currentSelectionCount >= maxSelectionCount)
            return false;

        // 武器条件
        if (!string.IsNullOrEmpty(requiredWeaponName))
        {
            if (slotManager.GetWeaponByName(requiredWeaponName) == null)
                return false;
        }

        // 标签条件
        foreach (var tag in requiredTags)
        {
            if (slotManager.GetWeaponCountByTag(tag) == 0)
                return false;
        }

        // 空槽位条件
        if (requireEmptySlot && slotManager.HasEmptySlot == false)
            return false;

        return true;
    }

    /// <summary>
    /// 获取基础权重（考虑稀有度）
    /// </summary>
    public float GetBaseWeight()
    {
        return (float)rarity;
    }

    /// <summary>
    /// 应用升级效果
    /// </summary>
    public void Apply(WeaponSlotManager slotManager, GameData gameData)
    {
        currentSelectionCount++;

        switch (effectType)
        {
            case UpgradeEffectType.StatBonus:
                ApplyStatBonus(slotManager);
                break;
            case UpgradeEffectType.NewWeapon:
                if (newWeapon != null)
                    slotManager.AcquireWeapon(newWeapon);
                break;
            //case UpgradeEffectType.Fragment:
                //slotManager.AddVirtualTag(fragmentTag);
                //break;
            case UpgradeEffectType.GlobalBonus:
                ApplyGlobalBonus(gameData);
                break;
        }
    }

    private void ApplyStatBonus(WeaponSlotManager slotManager)
    {
        // 如果有指定武器，只对该武器生效
        //if (!string.IsNullOrEmpty(requiredWeaponName))
        //{
        //    var weapon = slotManager.GetWeaponByName(requiredWeaponName);
        //    if (weapon != null)
        //    {
        //        weapon.damageBonus += damageBonus;
        //        weapon.attackSpeedBonus += attackSpeedBonus;
        //        weapon.projectileBonus += projectileBonus;
        //        weapon.pierceBonus += pierceBonus;
        //        weapon.bulletSpeedBonus += bulletSpeedBonus;
        //        weapon.bulletSizeBonus += bulletSizeBonus;
        //        weapon.rangeBonus += rangeBonus;
        //    }
        //}
        //else
        //{
            // 否则对所有武器生效
        //    foreach (var weapon in slotManager.GetAllWeapons())
        //    {
        //        weapon.damageBonus += damageBonus;
        //        weapon.attackSpeedBonus += attackSpeedBonus;
        //        weapon.projectileBonus += projectileBonus;
        //        weapon.pierceBonus += pierceBonus;
        //        weapon.bulletSpeedBonus += bulletSpeedBonus;
        //        weapon.bulletSizeBonus += bulletSizeBonus;
        //        weapon.rangeBonus += rangeBonus;
        //    }
        //}
    }

    private void ApplyGlobalBonus(GameData gameData)
    {
        gameData.AddDamageMultiplier(globalDamageBonus);
        gameData.expMultiplier += expMultiplierBonus;
        gameData.goldMultiplier += goldMultiplierBonus;
        gameData.AddMaxHealthBonus(maxHealthBonus);
        gameData.PlayerSpeedAdd(moveSpeedBonus);
        gameData.AddCriticalRate(critRateBonus);
        gameData.AddCriticalDamage(critDamageBonus);
        gameData.pickupRange += pickupRangeBonus;
    }


    /// <summary>
    /// 重置选择次数（新游戏时调用）
    /// </summary>
    public void ResetSelectionCount()
    {
        currentSelectionCount = 0;
    }
}

public enum UpgradeEffectType
    {
        StatBonus,      // 数值加成
        NewWeapon,      // 获得新武器
        Fragment,       // 羁绊碎片（增加虚拟标签）
        GlobalBonus,    // 全局加成
        MechanicChange  // 机制改变（如子弹反弹）
    }

    public enum Rarity
    {
        Common = 100,       // 普通
        Rare = 50,          // 稀有
        Epic = 20,          // 史诗
        Legendary = 5       // 传说
    }

