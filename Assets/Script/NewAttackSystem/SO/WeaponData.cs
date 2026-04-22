using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("=== 基础信息 ===")]
    public string weaponName = "新武器";
    [TextArea(2,4)]
    public string description = "";
    public Sprite icon;
    public EWeaponType weaponType = EWeaponType.Ranged;

    [Header("=== 标签系统（用于羁绊）===")]
    public List<EWeaponTag> tags = new List<EWeaponTag>();

    [Header("=== 等级成长（品质）===")]
    [Tooltip("最大等级（1=白, 5=红）")]
    public int maxLevel = 5;
    [Tooltip("伤害随等级成长的曲线")]
    public AnimationCurve damageCurve = AnimationCurve.Linear(1, 1, 5, 3);
    [Tooltip("攻速随等级成长的曲线")]
    public AnimationCurve attackSpeedCurve = AnimationCurve.Linear(1, 1, 5, 1.5f);
    [Tooltip("弹幕数随等级成长的曲线")]
    public AnimationCurve projectileCurve = AnimationCurve.Linear(1, 1, 5, 2);

    [Header("=== 子弹信息 ===")]
    public BulletBaseData bulletData;
    public GameObject bulletPrefab;

    [Header("=== 基础属性 ===")]
    [Tooltip("基础伤害")]
    public float baseDamage = 10f;
    [Tooltip("基础攻速（次/秒）")]
    public float baseAttackSpeed = 1f;
    [Tooltip("攻击范围（近战/激光用）")]
    public float baseRange = 3f;
    [Tooltip("基础弹幕数")]
    public int baseProjectileCount = 1;
    [Tooltip("散射角度（0 = 直线）")]
    public float spreadAngle = 0f;
    [Tooltip("攻击间隔随机偏移（让攻击节奏更自然）")]
    public float attackIntervalRandomness = 0.1f;

    [Header("=== 特殊效果 ===")]
    public EOnHitEffect onHitEffect = EOnHitEffect.None;
    [Tooltip("效果触发概率（0-1）")]
    public float effectChance = 0.3f;
    [Tooltip("效果持续时间（秒）")]
    public float effectDuration = 2f;
    [Tooltip("效果强度（如燃烧伤害倍数）")]
    public float effectPower = 0.5f;
    [Tooltip("特殊效果ID（用于代码判断）")]
    public string specialEffectId = "";

    [Header("=== 解锁条件 ===")]
    public bool unlockedByDefault = true;
    public int unlockCost = 0;
    public string unlockCondition = "";

    public float skillMultiplier = 1f;

    [HideInInspector]
    public float damagePercentBonus = 0f;
    [HideInInspector]
    public float attackSpeedBonus = 0f;
    [HideInInspector]
    public int projectileCountBonus = 0;
    [HideInInspector]
    public float bulletSpeedBonus = 0f;
    [HideInInspector]
    public float bulletSizeBonus = 0f;
    [HideInInspector]
    public int pierceBonus = 0;
    [HideInInspector]
    public int splitBonus = 0;
    [HideInInspector]
    public float explosionRadiusBonus = 0f; 


    /// <summary>
    /// 获取指定等级的伤害倍率
    /// </summary>
    public float GetDamageMultiplier(int level)
    {
        return damageCurve.Evaluate(Mathf.Clamp(level, 1, maxLevel));
    }

    /// <summary>
    /// 获取指定等级的攻速倍率
    /// </summary>
    public float GetAttackSpeedMultiplier(int level)
    {
        return attackSpeedCurve.Evaluate(Mathf.Clamp(level, 1, maxLevel));
    }

    /// <summary>
    /// 获取指定等级的弹幕数倍率
    /// </summary>
    public float GetProjectileMultiplier(int level)
    {
        return projectileCurve.Evaluate(Mathf.Clamp(level, 1, maxLevel));
    }

    /// <summary>
    /// 检查是否拥有指定标签
    /// </summary>
    public bool HasTag(EWeaponTag tag)
    {
        return tags.Contains(tag);
    }
}


public enum EWeaponType
{

    Melee,
    Ranged,
    Laser,
    AoE,
}
public enum EWeaponTag
{
    // 武器类型标签
    Gun,        // 枪械
    Melee,      // 近战
    Bow,        // 弓箭
    Magic,      // 魔法
    Summon,     // 召唤

    // 元素标签
    Fire,       // 火焰
    Ice,        // 冰霜
    Lightning,  // 雷电
    Poison,     // 毒素
    Dark,       // 暗影
    Light,      // 光明

    // 特性标签
    Physical,   // 物理
    Projectile, // 投射物
    Explosive,  // 爆炸
    Piercing,   // 穿透
    Homing      // 追踪
}

public enum EOnHitEffect
{
    None,
    Burn,       // 燃烧（持续伤害）
    Freeze,     // 冰冻（减速/定身）
    Shock,      // 感电（额外伤害/连锁）
    Poison,     // 中毒（持续伤害）
    Bleed,      // 流血（基于最大生命值伤害）
    Knockback,  // 击退
    Stun        // 眩晕
}