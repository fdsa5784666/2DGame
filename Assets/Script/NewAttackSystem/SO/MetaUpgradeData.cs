using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MetaUpgrade_", menuName = "Game/Meta Upgrade Data")]
public class MetaUpgradeData : ScriptableObject
{
    [Header("=== 显示信息 ===")]
    public string upgradeName = "新升级";
    [TextArea(2, 4)]
    public string description = "";
    public Sprite icon;
    public EMetaUpgradeCategory category = EMetaUpgradeCategory.General;

    [Header("=== 购买条件 ===")]
    public int cost = 100;
    public MetaUpgradeData prerequisite; // 前置升级
    public int requiredGamesWon = 0;

    [Header("=== 效果类型 ===")]
    public EMetaUpgradeEffectType effectType = EMetaUpgradeEffectType.StatBonus;

    [Header("=== 数值效果 ===")]
    public float damageBonus = 0f;
    public float healthBonus = 0f;
    public float speedBonus = 0f;
    public float expBonus = 0f;
    public float goldBonus = 0f;
    public float critRateBonus = 0f;
    public float critDamageBonus = 0f;
    public float pickupRangeBonus = 0f;
    [Tooltip("各种特殊加成的整数数值")]
    public int intValue;

    //[Header("=== 特殊效果 ===")]
    //public string specialEffectId = "";

    [Header("=== 可叠加 ===")]
    public bool stackable = false;
    public int maxStacks = 1;
    public float costMultiplierPerStack = 1.5f;

    // 运行时追踪
    [System.NonSerialized] public int currentStacks = 0;
    [System.NonSerialized] public bool isPurchased = false;

    public int GetCurrentCost()
    {
        return Mathf.RoundToInt(cost * Mathf.Pow(costMultiplierPerStack, currentStacks));
    }

    public bool CanPurchase()
    {
        if (isPurchased && !stackable) return false;
        if (stackable && currentStacks >= maxStacks) return false;
        if (prerequisite != null && !prerequisite.isPurchased) return false;
        if (MetaProgressionManager.Instance.GamesWon < requiredGamesWon) return false;
        return MetaProgressionManager.Instance.TotalGold >= GetCurrentCost();
    }

    public void Purchase()
    {
        if (!CanPurchase()) return;

        MetaProgressionManager.Instance.AddGold(-GetCurrentCost());
        currentStacks++;
        isPurchased = true;

        ApplyEffect();
    }

    private void ApplyEffect()
    {
        // 效果通过 MetaProgressionManager 统一管理
        MetaProgressionManager.Instance.ApplyMetaUpgrade(this);
    }
}

public enum EMetaUpgradeCategory
{
    General,        // 通用
    Offense,        // 攻击
    Defense,        // 防御
    Utility,        // 功能
    Character,      // 角色解锁
    Weapon          // 武器解锁
}

public enum EMetaUpgradeEffectType
{
    StatBonus,          // 数值加成
    UnlockCharacter,    // 解锁角色
    UnlockWeapon,       // 解锁武器
    ExtraSlot,          // 额外槽位
    StartingGold,       // 初始金币
    RerollUnlock,       // 解锁重选
    BanishUnlock        // 解锁剔除
}
