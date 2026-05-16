using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChooseClose : MonoBehaviour
{
    public GameObject panel;

    public void OnClick()
    {
        GameData.Instance.CancelCharacterBonus();
        GameData.Instance.SetCharacter(null);
        panel.SetActive(false);
    }
}
