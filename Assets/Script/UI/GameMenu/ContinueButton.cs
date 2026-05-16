using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public GameObject menu;

    public void ContinueButtonPress()
    {
            UIManager.instance.CloseAllPanel();
    }
}