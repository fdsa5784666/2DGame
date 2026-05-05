using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleModeButton : MonoBehaviour
{
    public void OnClick_SimpleMode()
    {
        SceneManager.Instance.SetSceneType(ESceneType.Game);
    }
}