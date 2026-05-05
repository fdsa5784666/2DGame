using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    private ESceneType currentSceneType;
    public GameObject panel_Game;
    public GameObject panel_Title;

    public event System.Action<ESceneType, ESceneType> OnSceneTypeChanged;
    public event System.Action OnGameStart; 

    public ESceneType CurrentSceneType => currentSceneType;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 0f;
        panel_Title.SetActive(true);
        panel_Game.SetActive(false);
    }

    public void SetSceneType(ESceneType newScene)
    {
        if (currentSceneType != newScene)
        {
            ESceneType old = currentSceneType;
            currentSceneType = newScene;
            OnSceneTypeChanged?.Invoke(old,newScene);
        }
    }
    /// <summary>
    /// 游戏重新开始 每次开始游戏调用一次 用于重置游戏状态
    /// </summary>
    public void ResetGame()
    {
        GameData.Instance.ResetSynergyBonuses();
        GameData.Instance.ResetLevel();
        EnemySpawner.Instance.ResetEnemySpawner();
        WeaponSlotManager.Instance.ResetWeaponSlot();
        UIManager.instance.ResetUI();
        OnGameStart?.Invoke();  //此处触发游戏开始事件，通知游戏即将开始，进行必要的重置

    }
}
public enum ESceneType
{
    Title,
    Game,
    GameOver,
}
