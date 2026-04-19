using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_Weapon_", menuName = "Weapon Upgrade Data")]
public class WeaponUpgradeData : ScriptableObject
{
    [Header("=== 显示信息 ===")]
    public string upgradeName = "武器强化";
    public string description = "";
    public Sprite icon;

    [Header("=== 目标武器 ===")]
    public string targetWeaponName;     // 针对哪个武器
    public bool isNewWeapon = false;    // 是否解锁新武器
    public WeaponData newWeaponData;    // 如果是新武器，填入数据

    [Header("=== 出现条件 ===")]
    public float weight = 100f;
    public int minPlayerLevel = 0;
    public int maxLevel = 20;

    [Header("=== 加成数值 ===")]
    public float damagePercentBonus = 0f;
    public float attackSpeedBonus = 0f;
    public int projectileCountBonus = 0;
    public float bulletSpeedBonus = 0f;
    public float bulletSizeBonus = 0f;
    public int pierceBonus = 0;
    public int splitBonus = 0;
    public float explosionRadiusBonus = 0f;
}