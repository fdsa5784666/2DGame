using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("=== 基础信息 ===")]
    public string weaponName = "新武器";
    [TextArea(2, 4)]
    public string description = "";
    public Sprite icon;
    public EWeaponType weaponType = EWeaponType.Ranged;

    [Header("=== 标签系统（用于羁绊）===")]
    public List<EWeaponTag> tags = new List<EWeaponTag>();

    [Header("=== 等级成长（品质）===")]
    [Tooltip("最大等级")]
    public int maxLevel = 3;
    [Tooltip("伤害随等级成长的曲线")]
    public AnimationCurve damageCurve = AnimationCurve.Linear(1, 1, 3, 3);
    [Tooltip("攻速随等级成长的曲线")]
    public AnimationCurve attackSpeedCurve = AnimationCurve.Linear(1, 1, 3, 1.5f);
    //    [Tooltip("弹幕数随等级成长的曲线")]
    //    public AnimationCurve projectileCurve = AnimationCurve.Linear(1, 1, 5, 2);


    [Header("=== 基础属性 ===")]
    [Tooltip("基础伤害")]
    public float baseDamage = 10f;
    [Tooltip("基础攻速（次/秒）")]
    public float baseAttackSpeed = 1f;
    [Tooltip("攻击范围（近战/激光用）")]
    public float baseAttackRange = 3f;

  

    public float GetDamage(int currentLevel) => baseDamage * damageCurve.Evaluate(Mathf.Clamp(currentLevel, 1, maxLevel));
    public float GetAttackSpeed(int currentLevel) => baseAttackSpeed * attackSpeedCurve.Evaluate(Mathf.Clamp(currentLevel, 1, maxLevel));
    public float GetAttackInterval(int currentLevel) => 1f / GetAttackSpeed(currentLevel);
}
public enum EWeaponType
{

    Melee,
    Ranged,
    Magic,
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

//public enum EOnHitEffect
//{
//    None,
//    Burn,       // 燃烧（持续伤害）
//    Freeze,     // 冰冻（减速/定身）
//    Shock,      // 感电（额外伤害/连锁）
//    Poison,     // 中毒（持续伤害）
//    Bleed,      // 流血（基于最大生命值伤害）
//    Knockback,  // 击退
//    Stun        // 眩晕
//}