using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public GameObject menu;

    public void ContinueButtonPress()
    {
        if (menu.activeSelf == false)
            UIManager.instance.OpenPanel(menu);
        else
            UIManager.instance.CloseAllPanel();
    }
}
