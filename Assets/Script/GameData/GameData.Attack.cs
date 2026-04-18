using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData : MonoBehaviour
{
    [Header("atk基础属性")]
    [SerializeField] private float baseAttack = 10f;
    [SerializeField] private float baseAttackSpeed = 1f;
    [SerializeField] private float baseCriticalRate = 0.05f;
    [SerializeField] private float baseCriticalDamage = 1.5f;

    [Header("atk百分比加成")]
    [SerializeField] private float attackPercentBonus = 0f;
    [SerializeField] private float attackSpeedPercentBonus = 0f;
    [SerializeField] private float criticalRateBonus = 0f;
    [SerializeField] private float criticalDamageBonus = 0f;
    [SerializeField] private float globalDamageMultiplier = 1f;

    [SerializeField]private float attackFlatBonus = 0f;
    public float FinalAttack => (baseAttack + attackFlatBonus) * (1 + attackPercentBonus);
    public float FinalAttackSpeed => baseAttackSpeed * (1 + attackSpeedPercentBonus);
    public float FinalCriticalRate => Mathf.Clamp01(baseCriticalRate +  criticalRateBonus);
    public float FinalCriticalDamage => baseCriticalDamage * (1 + criticalDamageBonus);

    public void AddAttackFlatBonus(float value) =>attackFlatBonus += value;
    public void AddAttackPercentBonus(float value) =>attackPercentBonus += value;
    public void AddAttackSpeedPercentBonus(float value) => attackSpeedPercentBonus += value;
    public void AddCritivalDamageBonus(float value) => criticalDamageBonus += value;
    public void AddCriticalRateBonus(float value) => criticalRateBonus += value;
    public void AddGlobalDamageMultiplier(float value) => globalDamageMultiplier += value;


    public float CalculateDamage(float skillMultiplier = 1f)
    {
        float damage = FinalAttack * skillMultiplier;
        if(Random.value < FinalCriticalRate) damage *= baseCriticalDamage;
        damage *= globalDamageMultiplier;
        damage *= Random.Range(0.95f, 1.05f);
        return Mathf.Max(1f,Mathf.Round(damage));
    }
}
