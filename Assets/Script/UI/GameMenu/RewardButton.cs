using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardButton : MonoBehaviour
{
    public TextMeshProUGUI totalKill;
    public TextMeshProUGUI gold;

    public GameObject rewardUI;
    public GameObject chooseReward;
    public GameObject quit;
    public void OnClick()
    {
        rewardUI.SetActive(true);
        chooseReward.SetActive(false);
        quit.SetActive(true);

        totalKill.text = "Killed X " + GameData.Instance.TotalKills.ToString();
        gold.text = "Gold X " + GameData.Instance.Gold.ToString();

        GameData.Instance.Reward();
    }

}
