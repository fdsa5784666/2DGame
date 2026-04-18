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

    [HideInInspector]
    //此选项当前选过多少次
    public int currentLevel = 0;
    public enum LevelUPType
    {
        playerSpeedUp,
        bulletSpeedUp,
        shootIntervalUp,
        AttackUp,
        HealthUp,
    }
    public LevelUPType type;
    public float value;

    public void Apply(GameObject GameData)
    {
        GameData gameData = GameData.GetComponent<GameData>();
        switch (type)
        {
            case LevelUPType.playerSpeedUp:
                gameData.playerSpeed += value;
                break;
            case LevelUPType.bulletSpeedUp:
                gameData.bulletSpeed += value;
                break;
            case LevelUPType.shootIntervalUp:
                gameData.shootInterval += value;
                gameData.shootInterval = Mathf.Clamp(gameData.shootInterval, 0.2f, 1f);
                break;
            case LevelUPType.AttackUp:
                gameData.bulletDamage += value; 
                break;
            case LevelUPType.HealthUp:
                gameData.playerMaxHealth += value;
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
