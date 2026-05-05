using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Synergy_", menuName = "Game/Synergy Data")]
public class SynergyData : ScriptableObject
{
    [Header("=== 羁绊信息 ===")]
    public string synergyName = "新羁绊";
    [TextArea(2, 4)]
    public string description = "";
    public Sprite icon;
    public Color themeColor = Color.white;
    public EWeaponTag requiredTag;

    [Header("=== 激活阈值 ===")]
    [Tooltip("激活羁绊所需的数量阈值，如 [2, 4, 6]")]
    public int[] thresholds = { 2, 4, 6 };

    [Header("=== 各阶段效果 ===")]
    public SynergyTierEffect[] tierEffects;

    [Header("=== 视觉效果（可选）===")]
    public GameObject activationEffectPrefab;
    public AudioClip activationSound;

    /// <summary>
    /// 获取下一阶段所需数量
    /// </summary>
    public int GetNextThreshold(int currentCount)
    {
        foreach (int threshold in thresholds)
        {
            if (threshold > currentCount)
                return threshold;
        }
        return -1; // 已满
    }

    /// <summary>
    /// 获取距离下一阶段还差多少
    /// </summary>
    public int GetMissingCount(int currentCount)
    {
        int next = GetNextThreshold(currentCount);
        if (next == -1) return 0;
        return next - currentCount;
    }

    /// <summary>
    /// 是否已满级
    /// </summary>
    public bool IsMaxed(int currentCount)
    {
        return GetNextThreshold(currentCount) == -1;
    }

    /// <summary>
    /// 获取当前阶段的描述
    /// </summary>
    public string GetCurrentTierDescription(int count)
    {
        int tier = GetActivatedTier(count);
        if (tier == -1) return "未激活";
        return tierEffects[tier].description;
    }

    /// <summary>
    /// 获取指定数量对应的激活阶段
    /// </summary>
    public int GetActivatedTier(int count)
    {
        int tier = -1;
        for (int i = 0; i < thresholds.Length; i++)
        {
            if (count >= thresholds[i])
                tier = i;
        }
        return tier;
    }

    /// <summary>
    /// 获取下一阶段的描述
    /// </summary>
    public string GetNextTierDescription(int currentCount)
    {
        int tier = GetActivatedTier(currentCount);
        int nextTier = tier + 1;
        if (nextTier >= thresholds.Length) return "已达最大等级";
        return tierEffects[nextTier].description;
    }

    /// <summary>
    /// 应用羁绊效果到游戏
    /// </summary>
    public void ApplyEffect(int tier, GameData gameData, WeaponSlotManager slotManager)
    {
        if (tier < 0 || tier >= tierEffects.Length) return;

        var effect = tierEffects[tier];
        effect.Apply(gameData, slotManager);
    }
}

[System.Serializable]
public struct SynergyTierEffect
{
    [Tooltip("阶段描述")]
    public string description;

    [Header("数值加成(百分比加成)")]
    public float damageBonus;
    public float attackSpeedBonus;
    public float critRateBonus;
    public float critDamageBonus;
    public float maxHealthBonus;
    public float moveSpeedBonus;

    [Header("特殊效果")]
    public string specialEffectId;
    public string specialEffectParams;

    public void Apply(GameData gameData, WeaponSlotManager slotManager)
    {
        gameData.AddDamageMultiplier(damageBonus);
        gameData.AddAttackSpeedBonus(attackSpeedBonus);
        gameData.AddCriticalRate(critRateBonus);
        gameData.AddCriticalDamage(critDamageBonus);
        //gameData.AddMaxHealthBonus(maxHealthBonus);
        //gameData.AddSpeedBonus(moveSpeedBonus);
    }
}
