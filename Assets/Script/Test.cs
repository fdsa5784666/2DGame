using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Button b;    
    public WeaponData data;
    void Start()
    {
        b.onClick.AddListener(OnClick);
    }

 private void OnClick()
    {
        WeaponSlotManager.Instance.AcquireWeapon(data);
    }
}
