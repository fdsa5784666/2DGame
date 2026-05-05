using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LevelUPOption
{
    public string optionName;
    public string description;

    public float weight = 100f;
    public int minPlayerLevel = 0;//玩家多少级出现此词条
    public int maxLevel = 10;//此选项最多被选择次数
    //public bool requierFullHealth = false;

    //[HideInInspector]
    //此选项当前选过多少次
    public int currentLevel = 0;
    public enum LevelUPType
    {
        playerSpeedUp,
        attackSpeedUp,
        AttackUp,
        HealthUp,
    }
    public LevelUPType type;
    public float value;

    public void Apply(GameObject GameData)
    {
        Debug.Log("Applying " + optionName);
        GameData gameData = GameData.GetComponent<GameData>();
        switch (type)
        {
            case LevelUPType.playerSpeedUp:
                gameData.playerSpeed += value;
                break;
            case LevelUPType.attackSpeedUp:
                gameData.AddAttackSpeedBonus(value);
                break;
            case LevelUPType.AttackUp:
                gameData.AddAttackFlat(value); 
                break;
            case LevelUPType.HealthUp:
                gameData.AddMaxHealthBonus(value);
                break;
            default:
                break;
        }

        currentLevel++;
    }
    public bool CanBeSelected(int playerLevel)
    {
        return playerLevel >= minPlayerLevel && currentLevel < maxLevel;
    }

}
