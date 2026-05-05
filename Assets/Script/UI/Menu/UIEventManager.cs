using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventManager : MonoBehaviour
{
    public GameObject panel_Game;
    public GameObject panel_Title;
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
            panel_Title.SetActive(false);
            panel_Game.SetActive(true);
            enemySpawner.SetActive(true);
            //playerMove.enabled = true;
            GameData.Instance.Resume();

        }
        //游戏中途返回标题界面 先进行结算
        else if (oldScene == ESceneType.Game && newScene == ESceneType.Title)
        {
            panel_Game.SetActive(false);
            panel_Title.SetActive(true);
            enemySpawner.SetActive(false);
            //playerMove.enabled = false;   
            SceneManager.Instance.ResetGame();
            GameData.Instance.Pause();
        }
    }

}
