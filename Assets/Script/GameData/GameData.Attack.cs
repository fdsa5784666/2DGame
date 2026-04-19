// GameData.Combat.cs
using UnityEngine;

public partial class GameData : MonoBehaviour
{
    [Header("=== 玩家通用战斗属性 ===")]
    [SerializeField] private float globalDamageMultiplier = 1f;
    [SerializeField] private float baseCriticalRate = 0.05f;
    [SerializeField] private float baseCriticalDamage = 1.5f;
    [SerializeField] private float criticalRateBonus = 0f;
    [SerializeField] private float criticalDamageBonus = 0f;

    public float FinalCriticalRate => Mathf.Clamp01(baseCriticalRate + criticalRateBonus);
    public float FinalCriticalDamage => baseCriticalDamage + criticalDamageBonus;
    public float GlobalDamageMultiplier => globalDamageMultiplier;

    // 全局属性修改
    public void AddCriticalRate(float v) => criticalRateBonus = Mathf.Clamp(criticalRateBonus + v, 0, 1 - baseCriticalRate);
    public void AddCriticalDamage(float v) => criticalDamageBonus += v;
    public void AddGlobalDamageMultiplier(float v) => globalDamageMultiplier += v;
}