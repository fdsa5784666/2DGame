using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    private ESceneType currentSceneType;
    public GameObject panel_Game;
    public GameObject panel_Title;
    public GameObject loadingUIObj;
    public Animator loadingUIAnimator;
    public AnimationClip loadingUIPlayClip;
    public AnimationClip loadingUIHideClip;

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

        loadingUIObj.SetActive(false);
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
        EnemySpawner.Instance.ResetEnemySpawner();
        WeaponSlotManager.Instance.ResetWeaponSlot();
        UIManager.instance.ResetUI();
        OnGameStart?.Invoke();  //此处触发游戏开始事件，通知游戏即将开始，进行必要的重置
        ObjectPool.Instance.ReturnAll();
        foreach(var obj in GameData.Instance.activeNonpooledObj)
        {
            Destroy(obj);
        }
        PlayerManager.Instance.ResetPlayerTransform();
        ResetAllParticles();
    }

    public IEnumerator LoadingUIPlay()
    {
        loadingUIObj.SetActive(true);
        loadingUIAnimator.SetTrigger("Play");
        yield return null;

        yield return new WaitForSecondsRealtime(loadingUIPlayClip.length);

    }
    public IEnumerator LoadingUIHide()
    {
        // 获取 AnimCallback
        var behaviours = loadingUIAnimator.GetBehaviours<AnimCallback>();
        AnimCallback callback = null;
        foreach (var b in behaviours)
        {
            if (b is AnimCallback)
            {
                callback = b as AnimCallback;
                break;
            }
        }

        bool completed = false;
        callback.OnComplete += () => completed = true;
        loadingUIAnimator.SetTrigger("Hide");
        yield return null;

        yield return new WaitUntil(() => completed);
        loadingUIObj.SetActive(false);

    }
    public void ResetAllParticles()
    {
        // 找到场景中所有的粒子系统（包括未激活的）
        ParticleSystem[] allParticles = FindObjectsOfType<ParticleSystem>(true);

        foreach (ParticleSystem ps in allParticles)
        {
            // 关键：清除所有已发射的粒子，并停止发射
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
public enum ESceneType
{
    Title,
    Game,
    GameOver,
}