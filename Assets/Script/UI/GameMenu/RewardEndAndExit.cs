using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardEndAndExit : MonoBehaviour
{

    public void OnClick()
    {
        SceneManager.Instance.SetSceneType(ESceneType.Title);
    }

}
