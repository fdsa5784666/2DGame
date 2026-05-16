using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipRewardAndQuit : MonoBehaviour
{

    public void OnClick()
    {
        GameData.Instance.Reward();

        SceneManager.Instance.SetSceneType(ESceneType.Title);

    }

}
