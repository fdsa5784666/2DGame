using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChooseComfirm : MonoBehaviour
{
    public GameObject panel;
    public void OnClick()
    {
        SceneManager.Instance.SetSceneType(ESceneType.Game);
        StartCoroutine(WaitSecClose());
    }
    IEnumerator WaitSecClose()
    {
        yield return new WaitForSecondsRealtime(1f);
        panel.SetActive(false);
    }
}
