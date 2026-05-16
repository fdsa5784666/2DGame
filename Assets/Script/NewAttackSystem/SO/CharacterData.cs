using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_",menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject   
{
    [Header("=== 基础信息 ===")]
    public string characterName = "新角色";
    [TextArea(2, 4)]
    public string description = "";
    public Sprite icon;
    public GameObject characterPrefab;

    [Header("=== 初始武器(锁定第一个槽位) ===")]
    public WeaponData startingWeapon;

    [Header("=== 初始属性加成 ===")]
    [Tooltip("最大生命值加成（百分比 1+x）")]
    public float maxHealthBonus = 0f;
    [Tooltip("移动速度加成（固定值）")]
    public float moveSpeedBonus = 0f;
    [Tooltip("伤害加成（百分比 1+x）")]
    public float damageBonus = 0f;
    [Tooltip("攻击速度加成（百分比）")]
    public float attackSpeedBonus = 0f;
    [Tooltip("暴击率加成（百分比）")]
    public float critRateBonus = 0f;

    [Header("=== 初始拥有武器（可选，除了起始武器外）===")]
    public List<WeaponData> additionalStartingWeapons;

    [Header("=== 角色专属升级池（为空则使用通用池）===")]
    public List<LevelUPOption> exclusiveUpgrades;

    [Header("=== 解锁条件 ===")]
    public bool unlockedByDefault = true;
    [Tooltip("解锁所需金币")]
    public int unlockCost = 0;
    [Tooltip("解锁条件描述（如：通关标准模式）")]
    public string unlockCondition = "";

    /// <summary>
    /// 应用角色初始属性到 GameManager
    /// </summary>
    public void ApplyToGameManager(bool Apply)
    {
        if (GameData.Instance == null)
        {
            Debug.LogError("GameData 不存在！");
            return;
        }

        var gm = GameData.Instance;

        if (Apply)
        {
            gm.AddMaxHealthBonus(maxHealthBonus);
            gm.SetCharacter(this);
            gm.playerCurrentHealth = gm.playerMaxHealth;
            gm.PlayerSpeedAdd(moveSpeedBonus);
            gm.AddDamageMultiplier(damageBonus);
            gm.AddAttackSpeedBonus(attackSpeedBonus);
            gm.AddCriticalRate(critRateBonus);
            Debug.Log("加成已应用");
        }
        else
        {
            gm.CancelCharacterBonus();
            gm.SetCharacter(null);
        }
    }

}

