using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_Attack", menuName = "Game/Upgrades/Attack Upgrade")]
public class BulletUpgradeData : ScriptableObject
{
    [Header("=== 显示信息 ===")]
    public string upgradeName = "攻击力强化";
    public string description = "攻击力 +15%";
    public Sprite icon;

    [Header("=== 出现条件 ===")]
    public float weight = 100f;
    public int minPlayerLevel = 0;
    public int maxLevel = 20;

    [Header("=== 加成数值 ===")]
    [Tooltip("攻击力百分比加成")]
    public float attackPercentBonus = 0.15f;

    [Tooltip("子弹速度加成")]
    public float bulletSpeedBonus = 0f;

    [Tooltip("子弹大小加成")]
    public float bulletSizeBonus = 0f;

    [Tooltip("穿透次数增加")]
    public int pierceBonus = 0;

    [Tooltip("弹幕数量增加")]
    public int projectileCountBonus = 0;

    [Tooltip("分裂数量增加")]
    public int splitBonus = 0;

    [Tooltip("爆炸半径增加")]
    public float explosionRadiusBonus = 0f;
}