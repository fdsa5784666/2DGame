using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject menu;
    public Button menuButton;
    public Transform Text1;
    public Transform Text2;
    public Transform Text3;

    private TextMeshProUGUI Text1_UI;
    private TextMeshProUGUI Text2_UI;
    private TextMeshProUGUI Text3_UI;
    private RectTransform Text1_Rect;
    private RectTransform Text2_Rect;
    private RectTransform Text3_Rect;

    private Stack<GameObject> panelStack = new Stack<GameObject>();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        menu.SetActive(false);
        GameData.Instance.PauseSignal.SetActive(false);

        Text1_Rect = Text1.GetComponent<RectTransform>();
        Text2_Rect = Text2.GetComponent<RectTransform>();
        Text3_Rect = Text3.GetComponent<RectTransform>();

        Text1_UI = Text1.gameObject.GetComponent<TextMeshProUGUI>();
        Text2_UI = Text2.gameObject.GetComponent<TextMeshProUGUI>();
        Text3_UI = Text3.gameObject.GetComponent<TextMeshProUGUI>();

        menuButton.onClick.AddListener(OnButtonClick_Menu);
    }
    void Update()
    {
        MenuController();
    }

    public void MenuController()
    {
        if ( Input.GetKeyDown(KeyCode.Escape) )
        {
            CloseTopPanel();
        }
        
        if (RectTransformUtility.RectangleContainsScreenPoint(Text1_Rect, Input.mousePosition))
        {
            Text1_UI.fontStyle |= FontStyles.Underline;
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(Text2_Rect, Input.mousePosition))
        {
            Text2_UI.fontStyle |= FontStyles.Underline;
        }
        else if ( RectTransformUtility.RectangleContainsScreenPoint(Text3_Rect, Input.mousePosition) )
        {
            Text3_UI.fontStyle |= FontStyles.Underline;
        }
        else
        {
            //Text1_UI.fontStyle &= FontStyles.Underline;
            //Text2_UI.fontStyle &= FontStyles.Underline;
            //Text3_UI.fontStyle &= FontStyles.Underline;

            Text1_UI.fontStyle = FontStyles.Normal;
            Text2_UI.fontStyle = FontStyles.Normal;
            Text3_UI.fontStyle = FontStyles.Normal;
        }

    }
        
    void OnButtonClick_Menu()
    {
        if(menu.activeSelf == false)
            OpenPanel(menu);
        else
            CloseAllPanel();
    }

    public void OpenPanel(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }
        panel.SetActive(true);
        if (panelStack.Count == 0)
        {
            GameData.Instance.Pause();
        }

        panelStack.Push(panel);
    }
    public void CloseTopPanel()
    {
        if (panelStack.Count > 1)
        {
            GameObject topPanel = panelStack.Pop();
            topPanel.SetActive(false);
        }
        else if(panelStack.Count == 1) 
        {
                GameObject topPanel = panelStack.Pop();
                topPanel.SetActive(false);
                GameData.Instance.Pause();
        }


    }
    public void CloseAllPanel ()
    {
        while (panelStack.Count > 0)
        {
            panelStack.Pop().SetActive(false);
            GameData.Instance.Pause();
        }
    }
}

