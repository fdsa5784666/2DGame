using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventManager : MonoBehaviour
{
    public GameObject panel_Game;
    public GameObject panel_Title;
    public GameObject panel_GameEnd;
    public GameObject enemySpawner;
    //public PlayerMove playerMove;
    


    private void Start()
    {
        if(panel_Game == null || panel_Title == null)
        {
            Debug.LogError($"检查{this.name}的面板引用是否正确");
            return;
        }
        SceneManager.Instance.OnSceneTypeChanged += SceneSwitch;

    }
    private void SceneSwitch(ESceneType oldScene, ESceneType newScene)
    {
        //游戏开始 应进行必要设置
        if (oldScene == ESceneType.Title && newScene == ESceneType.Game)
        {
            StartCoroutine(TransitionToGame());
        }
        //游戏中途返回标题界面 先进行结算
        else if (oldScene == ESceneType.Game && newScene == ESceneType.Title)
        {
            Debug.Log("游戏界面切换到标题界面");
            panel_Game.SetActive(false);
            panel_Title.SetActive(true);
            panel_GameEnd.SetActive(false);
            enemySpawner.SetActive(false);

            //playerMove.enabled = false;   
            SceneManager.Instance.ResetGame();
            GameData.Instance.Pause();
        }
    }
    IEnumerator TransitionToGame()
    {
        // 1. 播放 Loading 动画入场
        yield return StartCoroutine(SceneManager.Instance.LoadingUIPlay());
        // 2. 等动画播稳了，切面板（玩家看不到）
        panel_Title.SetActive(false);
        panel_Game.SetActive(true);
        enemySpawner.SetActive(true);
        GameData.Instance.InitializeGame();
        UIManager.instance.ResetUI();
        // 3. 再等一小会，让 Loading 动画播完退场
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(SceneManager.Instance.LoadingUIHide());
        // 4. 正式开始
        GameData.Instance.Resume();
    }
}
