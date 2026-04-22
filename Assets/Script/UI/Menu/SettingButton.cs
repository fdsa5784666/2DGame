using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public GameObject setting;
    public Button closeButton;
    private void Start()
    {
        if (setting != null)
        {
            setting.SetActive(false);
        }
        closeButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick_Setting()
    {
        if (setting != null)
        {
            if (setting.activeSelf == false)
                UIManager.instance.OpenPanel(setting);
            else
                UIManager.instance.CloseTopPanel();
        }
    }
    public void OnButtonClick()
    {
        UIManager.instance.CloseTopPanel();
    }
}
