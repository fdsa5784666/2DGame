using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterChooseButtonController : MonoBehaviour
{
    public List<Button> tabButtons;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.gray;

    private Button currentSelected;

    void Start()
    {
        foreach (var btn in tabButtons)
        {
            // 用局部变量避免闭包问题
            Button capturedBtn = btn;
            btn.onClick.AddListener(() => OnTabClicked(capturedBtn));

            // 初始全部设为普通色
            ColorBlock cb = btn.colors;
            cb.normalColor = normalColor;
            btn.colors = cb;
        }
    }

    void OnTabClicked(Button clicked)
    {
        // 如果点的是已经高亮的 → 取消高亮
        if (currentSelected == clicked)
        {
            DeselectCurrent();
            // 这里通知角色系统取消加成
            return;
        }

        // 全部还原
        foreach (var btn in tabButtons)
        {
            SetButtonNormal(btn);
        }

        // 高亮当前
        SetButtonHighlighted(clicked);
        currentSelected = clicked;

        // 这里通知角色系统切换加成
    }

    // 取消当前选中
    public void DeselectCurrent()
    {
        if (currentSelected != null)
        {
            SetButtonNormal(currentSelected);
            currentSelected = null;
        }
    }

    void SetButtonNormal(Button btn)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = normalColor;
        btn.colors = cb;
    }

    void SetButtonHighlighted(Button btn)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = selectedColor;
        btn.colors = cb;
    }
}