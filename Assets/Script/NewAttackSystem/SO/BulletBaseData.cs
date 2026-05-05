using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet_",menuName = "Game/Bullet Base Data")]
public class BulletBaseData : ScriptableObject
{
    [Header("=== 基础属性 ===")]
    [Tooltip("子弹基础伤害")]
    public float damage = 10f;
    [Tooltip("子弹飞行速度")]
    public float speed = 8f;
    [Tooltip("子弹存活时间（秒）")]
    public float lifeTime = 3f;
    [Tooltip("子弹基础大小")]
    public float size = 1f;

    [Header("=== 特殊能力 ===")]
    [Tooltip("可穿透敌人数量(1 = 不穿透)")]
    public int pierceCount = 1;

    [Tooltip("是否追踪敌人")]
    public bool homing = false;
    [Tooltip("追踪转向强度(值越大转弯越快)")]
    public float homingStrength = 2f;
    [Tooltip("追踪搜索范围")]
    public float homingSearchRange = 10f;

    [Tooltip("命中后分裂的子弹数量(0 = 不分裂)")]
    public int splitCount = 0;
    [Tooltip("分裂子弹的散射角度")]
    public float splitAngle = 30f;
    [Tooltip("分裂子弹伤害倍率")]
    public float splitDamageMultiplier = 0.5f;

    [Tooltip("爆炸半径 (0 = 不爆炸)")]
    public float explosionRadius = 0f;
    [Tooltip("爆炸伤害倍率(基于子弹伤害)")]
    public float explosionDamageMultiplier = 0.7f;

    [Header("=== 反弹（可选）===")]
    [Tooltip("墙壁反弹次数（0 = 不反弹）")]
    public int bounceCount = 0;
    [Tooltip("反弹后是否重新索敌")]
    public bool retargetOnBounce = true;

    [Header("=== 视觉效果 ===")]
    public Sprite bulletSprite;
    public Color bulletColor = Color.white;
    public Material bulletMaterial;

    [Tooltip("是否使用渐变颜色")]
    public bool useGradient = false;
    public Gradient colorGradient;

    [Header("=== 特效预制体 ===")]
    public GameObject hitEffectPrefab;
    public GameObject trailEffectPrefab;
    public GameObject criticalHitEffectPrefab;
}
