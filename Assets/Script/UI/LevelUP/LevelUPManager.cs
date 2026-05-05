using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Sprites;
using UnityEngine;

public class LevelUPManager : MonoBehaviour
{
    //public List<BulletUpgradeData> allPossibleUpgrades;
    public List<LevelUPOption> allPossibleLevelUP;             //原本加成 需要手动添加
    public GameObject levelUPPanelPrefab;
    public Canvas mainCanvas;
    private GameObject currentPanel;

    [Header("随机规则")]
    [SerializeField]
    private bool allowDuplicateOptions = false;

    public void ShowRandowUpgrades()
    {
        GameData.Instance.Pause();

        List<LevelUPOption> selected = GetRandomUpgrades(3);

        currentPanel = Instantiate(levelUPPanelPrefab, mainCanvas.transform);

        currentPanel.GetComponent<LevelUPSelectionUI>().Setup(selected, this);
    }
    /// <summary>
    /// 根据权重抽取count个可选升级选项
    /// </summary>
    /// <param name = "count" ></ param >
    /// < returns ></ returns >
    public List<LevelUPOption> GetRandomUpgrades(int count)
    {
        //获取可用项
        int playerLevel = GetPlayerLevel();
        List<LevelUPOption> availableOption = allPossibleLevelUP.
        Where(opt => opt != null && opt.CanBeSelected(playerLevel)).ToList();
        //没有就结束
        if (availableOption.Count == 0)
        {
            Debug.Log("没有可用升级选项");
            return new List<LevelUPOption>();
        }

        //数量正好或者不足就打乱全部取出
        if (availableOption.Count <= count)
        {
            return ShuffleList(availableOption);
        }

        //正常情况: 按权重抽取
        List<LevelUPOption> selected = new List<LevelUPOption>();
        List<LevelUPOption> pool = new List<LevelUPOption>(availableOption);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            LevelUPOption picked = PickOne(pool);
            selected.Add(picked);
            //如果不可重复 从复制的总池里删掉
            if (!allowDuplicateOptions)
            {
                pool.Remove(picked);
            }

        }
        return selected;
    }

    /// <summary>
    /// 权重总和的范围内选一个值 并逐个比对在哪个区间
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private LevelUPOption PickOne (List<LevelUPOption> options)
    {
        float totalWeight = 0f;
        foreach(LevelUPOption opt in options)
        {
            totalWeight += opt.weight;
        }

        float randomValue = Random.Range(0, totalWeight);

        float cumulativeWeight = 0f;
        foreach (LevelUPOption opt in options)
        {
            cumulativeWeight += opt.weight;
            if(randomValue <= cumulativeWeight)
            {
                return opt;
            }
        }
        return options[options.Count - 1];
    }
    /// <summary>
    /// 打乱列表
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private List<LevelUPOption> ShuffleList(List<LevelUPOption> list)
    {
        List<LevelUPOption> shuffled = new List<LevelUPOption>(list);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            LevelUPOption temp = shuffled[i];
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }
        return shuffled;
    }

    public void OnOptionSelected(LevelUPOption chosen)
    {
        //GameData.Instance.AddAttackPercent(chosen.attackPercentBonus);
        //GameData.Instance.AddBulletSpeed(chosen.bulletSpeedBonus);
        //GameData.Instance.AddBulletSize(chosen.bulletSizeBonus);
        //GameData.Instance.AddPierce(chosen.pierceBonus);
        //GameData.Instance.AddProjectileCount(chosen.projectileCountBonus);
        //GameData.Instance.AddSplitCount(chosen.splitBonus);
        //GameData.Instance.AddExplosionRadius(chosen.explosionRadiusBonus);
        chosen.Apply(GameData.Instance.gameObject);
        GameData.Instance.Resume();
        GameData.Instance.FullHP();

        //此处为选择升级效果的最后 在此加入特效


        Destroy(currentPanel);

    }
    /// <summary>
    /// 获取玩家等级
    /// </summary>
    /// <returns></returns>
    private int GetPlayerLevel()
    {
        return GameData.Instance.playerLevel;
    }
}
