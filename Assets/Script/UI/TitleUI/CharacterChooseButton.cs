using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterChooseButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    [Header("角色的SO")]
    public CharacterData characterData;

    public void OnClick()
    {
        if (characterData == null)
        {
            Debug.LogWarning($"{nameText.text} 没有绑定 CharacterData");
            return;
        }
        var gm = GameData.Instance;
        var current = gm.PlayerChosenCharacter;

        if (current != null && current.characterName == nameText.text)
        {
            characterData.ApplyToGameManager(false);
        }
        else
        {
            GameData.Instance.CancelCharacterBonus();

            // 应用当前角色加成
            characterData.ApplyToGameManager(true);
        }
    }



}
