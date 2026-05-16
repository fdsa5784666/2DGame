using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    #region UI初始化需要的变量
    [Header("UI初始化需要的变量")]
    public GameObject pauseSignal;
    public GameObject gameMode;
    [Tooltip("结算选择")]
    public GameObject button;
    [Tooltip("按结算后出现的那个退出键")]
    public GameObject button1;
    [Tooltip("结算用的那个展示UI")]
    public GameObject button2;
    [Tooltip("选择结束要按的那个按钮")]
    public Button button3;
    #endregion

    #region 游戏内菜单变量
    [Header("游戏内菜单变量")]
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

    public GameObject panel_GameOver;
    [Tooltip("结束界面显示的文字")]
    public TextMeshProUGUI GameOverPanelText;
    #endregion
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
        if (SynergyManager.Instance == null) Debug.LogError("instance未赋值");
        SynergyManager.Instance.OnSynergyUpdated += ShowSynergySignal;

        GameData.Instance.OnCharacterChosen += EnableButton;
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
        Debug.Log($"当前UI数量是{panelStack.Count}");
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
            panelStack.Pop().SetActive(false);
        }
        else if(panelStack.Count == 1) 
        {
            panelStack.Pop().SetActive(false);
            GameData.Instance.Resume();
        }


    }
    public void CloseAllPanel ()
    {
        while (panelStack.Count > 0)
        {
            CloseTopPanel();
        }
    }

    /// <summary>
    /// 此函数应展示游戏结束UI 少量不同表示场次结束情况
    /// </summary>
    /// <param name="virtory"></param>
    public void ShowGameOver(bool victory)
    {
        if (victory)
        {
            GameOverPanelText.text = "Victory";
            panel_GameOver.SetActive(true);
            Debug.Log("玩家胜利,游戏结束");
        }
        else
        {
            GameOverPanelText.text = "Game Over";
            panel_GameOver.SetActive(true);
            Debug.Log("玩家失败,游戏结束");
        }
    }
    
    public void ShowSynergySignal(SynergyConfig config,int tier)
    {
        Debug.Log($"触发了{config.name}的{tier + 1}级增益");
    }
    /// <summary>
    /// 激活那个角色选择后的按钮
    /// </summary>
    public void EnableButton (bool IsChosen)
    {
        if (IsChosen)
        {
            button3.interactable = true;
        }
        else
            button3.interactable= false;
    }

    public void ResetUI()
    {
        CloseAllPanel();
        pauseSignal.SetActive(false);
        gameMode.SetActive(false);
        button.SetActive(true);
        button1.SetActive(false);
        button2.SetActive(false);
    }
}

