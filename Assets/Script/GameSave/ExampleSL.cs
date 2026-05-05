//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class ExampleSL : MonoBehaviour
//{
//    public TextMeshProUGUI goldText;
//    public GameSave save;
//    void Start()
//    {
//        save = SaveManager.Instance.GetCurrentSave();
//        Debug.Log("111");
//        if(SaveManager.Instance == null)
//        {
//            Debug.LogError("请确保场景中有SaveManager实例");
//            return;
//        }
//        // 方式1：直接访问
//        //int gold = SaveManager.Instance.GetCurrentSave().gold;
//        //goldText.text = gold.ToString();

//        //方式2：监听加载完成

//        if (SaveManager.Instance.IsSaveLoaded)
//        {
//            Debug.Log("SaveManager已经可用");
//            AddGoldOnGameEnd(100);
//            Invoke(nameof(asd), 2f); // 延迟调用，确保UI更新
//        }
//        else
//        {
//            Debug.Log("SaveManager尚未加载完成，监听事件");
//            //SaveManager.Instance.OnSaveLoaded += OnSaveLoaded;
//        }
//    }

//    //void OnSaveLoaded(GameSave save)
//    //{
//    //    if (goldText == null)
//    //    {
//    //        Debug.Log("缺少组件");
//    //        return;
//    //    }
//    //    goldText.text = save.gold.ToString();
//    //    //AddGoldOnGameEnd(100);
//    //}
//    void asd()
//    {
//        goldText.text = save.gold.ToString();

//    }

//    //结算时增加金币
//    public void AddGoldOnGameEnd(int earnedGold)
//    {
//        SaveManager.Instance.Modify(save =>
//        {
//            save.gold += earnedGold;
//            save.totalGamesPlayed++;
//        });
//    }
//}
