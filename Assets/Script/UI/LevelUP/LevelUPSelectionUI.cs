using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUPSelectionUI : MonoBehaviour
{
    public Button[] optionButtons;
    public TextMeshProUGUI[] optionNames;
    public TextMeshProUGUI[] optionDescriptions;

    private List<LevelUPOption> currentOptions;
    private LevelUPManager manager;

    public void Setup(List<LevelUPOption>options,LevelUPManager mgr)
    {
        currentOptions = options;
        manager = mgr;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Count)
            {
                optionNames[i].text = options[i].optionName;
                optionDescriptions[i].text = options[i].description;

                int index = i; // 闭包问题
                optionButtons[i].onClick.AddListener(() => OnButtonClick(index));
                optionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnButtonClick(int index)
    {
        manager.OnOptionSelected(currentOptions[index]);
    }

}
