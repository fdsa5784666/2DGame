using UnityEngine;

[System.Serializable]
public struct BulletInfo
{
    // 战斗属性
    public float damage;
    public float speed;
    public float size;
    public float lifetime;

    // 特殊能力
    public int pierceCount;
    public bool homing;
    public float homingStrength;
    public int splitCount;
    public float splitAngle;
    public float explosionRadius;
    public float explosionDamageMultiplier;

    // 视觉效果
    public Color bulletColor;
    public GameObject hitEffectPrefab;
    public GameObject trailEffectPrefab;

    // 元数据
    public bool isCritical;
}