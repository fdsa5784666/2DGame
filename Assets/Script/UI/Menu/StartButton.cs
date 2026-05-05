using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public GameObject ModeSelect;

    private void Start()
    {
        ModeSelect.SetActive(false);
    }
    public void OnClick_Start()
    {
        if (ModeSelect.activeSelf == false)
        {
            ModeSelect.SetActive(true);
        }
        else
        {
            ModeSelect.SetActive(false);
        }
    }
}
