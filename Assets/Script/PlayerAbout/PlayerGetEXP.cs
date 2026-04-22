using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGetEXP : MonoBehaviour
{
    public Image EXPView;
    public TextMeshProUGUI Text;
    public LevelUPManager LevelUPManager;
    private float lastTargetLevelFloat = 0f;
    private float targetLevelFloat = GameData.playerFirstUpgradeValue;
    private float nowLevelEXP;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EXP"))
        {
            Debug.Log("触发EXPtag");
            IPickable_EXP pickEXP = collision.GetComponent<IPickable_EXP>();
            GetEXP(pickEXP.PickEXP());

            IPooledable pooledable = collision.GetComponent<IPooledable>();
            pooledable.ReturnToPool();
        }
    }

    public void GetEXP( int EXP )
    {
        GameData.Instance.playerEXP += EXP;

        float playerLevelFloat = (float)GameData.Instance.playerEXP;
        if (playerLevelFloat >= targetLevelFloat)
        {
            PlayerLevelUP();
        }
    }

    public void PlayerLevelUP()
    {
        GameData.Instance.playerLevel++;


        Debug.Log($"玩家升级了,现在{GameData.Instance.playerLevel}");
        //此处为升级选项UI的开始 可在此处加入特效
        LevelUPManager.ShowRandowUpgrades();


        GameData.Instance.expToNextLevel = (int)(Mathf.Round(GameData.Instance.expToNextLevel *
                                                 GameData.LevelUpValueMultValue));
        lastTargetLevelFloat = targetLevelFloat;
        targetLevelFloat += GameData.Instance.expToNextLevel;

    }

    private void Update()
    {
        RefreshHPUI();
    }

    void RefreshHPUI()
    {
        if (GameData.Instance != null)
        {
            nowLevelEXP = (float)GameData.Instance.playerEXP - lastTargetLevelFloat;

        }
        EXPView.fillAmount = nowLevelEXP /
            (float)GameData.Instance.expToNextLevel;
        Text.text = $"Level {GameData.Instance.playerLevel}  " +
            $"{nowLevelEXP}/{GameData.Instance.expToNextLevel}EXP";

        //使用总值表示经验条 
        //但无法表示目前等级百分比
        //EXPView.fillAmount = GameData.Instance.playerEXP / targetLevelFloat;
        //Text.text = $"Level {GameData.Instance.playerLevel}  " +
        //            $"{GameData.Instance.playerEXP}/{targetLevelFloat}EXP";

    }
}
