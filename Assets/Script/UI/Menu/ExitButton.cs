using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public GameObject menu;

    public void ExitButtonPress()
    {
        Debug.Log("返回标题界面");
        GameData.Instance.GameOver(false);
        SceneManager.Instance.SetSceneType(ESceneType.Title);
    }
}
