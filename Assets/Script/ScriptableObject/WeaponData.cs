using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("=== 基础信息 ===")]
    public string weaponName = "新武器";
    public string description = "";
    public Sprite icon;
    public EWeaponType weaponType;

    [Header("=== 子弹信息 ===")]
    public BulletBaseData bulletData;

    [Header("=== 攻击参数 ===")]
    public float baseDamage = 10f;
    public float attackSpeed = 1f;  
    public float skillMultiplier = 1f;
    public int projectileCount = 1;
    public float spreadAngle = 0f; // 扩散角度，单位为度

    [Header("=== 升级加成 ===")]
    public float damagePercentBonus = 0f;
    public float attackSpeedBonus = 0f;
    public int projectileCountBonus = 0;
    public float bulletSpeedBonus = 0f;
    public float bulletSizeBonus = 0f;
    public int pierceBonus = 0;
    public int splitBonus = 0;
    public float explosionRadiusBonus = 0f; 
}

public enum EWeaponType
{

    Melee,
    Ranged,
    Laser,
    AoE,
}
