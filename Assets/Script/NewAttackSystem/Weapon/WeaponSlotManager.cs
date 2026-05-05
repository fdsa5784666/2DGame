using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WeaponSlotManager : MonoBehaviour
{
    public static WeaponSlotManager Instance { get; private set; }

    [Header("=== 槽位配置 ===")]
    [SerializeField] private int maxWeaponSlot = 6;
    [SerializeField] private Transform weaponAttackPoint;
    [SerializeField] private List<WeaponInstance> equippedWeapons = new List<WeaponInstance>();
    // 事件
    public event System.Action<WeaponData, int> OnWeaponAdded;            //武器 新等级
    public event System.Action<WeaponData, int> OnWeaponUpgraded;        //武器 新等级
    public event System.Action<int> OnWeaponRemoved;                         //槽位索引
    public event System.Action<WeaponData> OnWeaponReplacedRequired;        //需要替换武器

    public bool HasEmptySlot => equippedWeapons.Count < maxWeaponSlot;
    public IReadOnlyList<WeaponData> EquippedWeapons => equippedWeapons.Select(w => w.Data).ToList();
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 初始化标签字典
        //foreach (EWeaponTag tag in System.Enum.GetValues(typeof(EWeaponTag)))
        //{
        //    weaponTagCounts[tag] = 0;
        //    virtualTagCounts[tag] = 0;
        //}
    }
    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var weapon in equippedWeapons)
        {
            // 这里可以添加武器的持续效果更新逻辑
            // 例如：某些武器可能有持续伤害加成，需要每帧更新
        }
    }
    /// <summary>
    /// 获得武器
    /// </summary>
    public bool AcquireWeapon(WeaponData data)
    {
        //var existing = equippedWeapons.Find(w => w.Data == data);

        ////已拥有
        //if (existing != null)
        //{
        //    existing.LevelUp();
        //    OnWeaponUpgraded?.Invoke(data, existing.StarLevel);
        //    Debug.Log($"{data.weaponName} 升级到{existing.StarLevel}级");
        //    return true;
        //}
        ////判满
        //if (equippedWeapons.Count >= maxWeaponSlot)
        //{
        //    Debug.Log("武器槽已满，无法获得，需要先替换或者移除");
        //    return false;
        //}
        ////新增
        //AddNewWeapon(data);
        //return true;
        if (!TryAddWeaponInstance(data, out WeaponInstance newWeapon)) return false;

        CheckAndCombineAll();

        return true;


    }
    private bool TryAddWeaponInstance(WeaponData data ,out WeaponInstance newWeapon)
    {
        newWeapon = null;

        if(equippedWeapons.Count < maxWeaponSlot)
        {
            newWeapon = CreateWeaponInstance(data, starLevel: 1);
            equippedWeapons.Add(newWeapon);
            OnWeaponAdded?.Invoke(data, 1);
            Debug.Log($"获得新武器: {data.weaponName}(1星)");
            return true;

        }

        Debug.Log("武器槽已满");
        OnWeaponReplacedRequired?.Invoke(data);
        return false;
    }

    /// <summary>
    /// 替换武器
    /// </summary>
    public void ReplaceWeapon(int slotIndex, WeaponData newWeaponData)
    {
        if (slotIndex < 0 || slotIndex >= equippedWeapons.Count)
        {
            Debug.LogError("无效的槽位索引");
            return;
        }

        var oldWeapon = equippedWeapons[slotIndex];
        oldWeapon.Destroy();
        equippedWeapons.RemoveAt(slotIndex);

        var newWeapon = CreateWeaponInstance(newWeaponData, 1);
        equippedWeapons.Insert(slotIndex, newWeapon);

        OnWeaponRemoved?.Invoke(slotIndex);
        OnWeaponAdded?.Invoke(newWeaponData, 1);
        Debug.Log($"替换武器:槽位{slotIndex} -> {newWeaponData.weaponName}(1星)");

        CheckAndCombineAll();
    }
    public void DiscardWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedWeapons.Count)
        {
            Debug.LogError("无效的槽位索引");
            return;
        }

        var weapon = equippedWeapons[slotIndex];
        string weaponName = weapon.Data.weaponName; 
        weapon.Destroy();
        equippedWeapons.RemoveAt(slotIndex);
        OnWeaponRemoved?.Invoke(slotIndex);

        Debug.Log($"丢弃武器:槽位{slotIndex} -> {weaponName}");

    }
    /// <summary>
    /// 增加武器槽位
    /// </summary>
    /// <param name="increment">增加数量</param>
    public void IncreaseMaxSlots(int increment)
    {
        maxWeaponSlot += increment;
        Debug.Log($"增加武器槽位:当前最大槽位数 {maxWeaponSlot}");
    }
    //========= 核心逻辑 ========
    /// <summary>
    /// 尝试合成所有可合成物品
    /// </summary>
    private void CheckAndCombineAll()
    {
        bool anyCombined = false;
        do
        {
            anyCombined = TryCombineOnce();
        }
        while (anyCombined);
    }
    
    private bool TryCombineOnce()
    {
        var groups = equippedWeapons
            .Where(w => !w.IsMaxStar)
            .GroupBy(w => (w.Data, w.StarLevel))
            .Where(g => g.Count() >= 3)
            .ToList();

        if(groups.Count == 0) return false;

        var group = groups[0];
        var weaponData = group.Key.Data;
        int currentStar = group.Key.StarLevel;
        var threeWeapons = group.Take(3).ToList();

        foreach(var w in threeWeapons)
        {
            Debug.Log($"预合成检查 | 武器:{w.Data.weaponName} 星级:{w.StarLevel} Data实例ID:{w.Data.GetInstanceID()}");
            equippedWeapons.Remove(w);
            w.Destroy();
        }

        int newStar = currentStar + 1;  
        var newWeapon = CreateWeaponInstance(weaponData, newStar);
        equippedWeapons.Add(newWeapon);

        OnWeaponUpgraded?.Invoke(weaponData, newStar);
        Debug.Log($"3个{weaponData.weaponName}({currentStar}星) -> 1个{weaponData.weaponName}({newStar}星)");

        return true;
       }
    
    /// <summary>
    /// 获取指定标签的所有武器
    /// </summary>
    public List<WeaponInstance> GetWeaponsByTag(EWeaponTag tag)
    {
        return equippedWeapons.Where(w => w.Data.tags.Contains(tag)).ToList();
    }
    /// <summary>
    /// 获取指定标签的数量
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public int GetWeaponCountByTag(EWeaponTag tag)
    {
        return equippedWeapons.Count(w => w.Data.tags.Contains(tag));
    }
    public int GetWeaponCount(WeaponData data,int starLevel)
    {
        return equippedWeapons.Count(w => w.Data == data && w.StarLevel == starLevel);
    }
    public WeaponInstance GetWeaponByName(string weaponName)
    {
        return equippedWeapons.Find(w => w.Data.weaponName == weaponName);
    }
    public void ResetWeaponSlot()
    {
        equippedWeapons.Clear(); 
    }
    // ======= 私有方法 ======

    private WeaponInstance CreateWeaponInstance(WeaponData data, int starLevel)
    {
        GameObject weaponObj = new GameObject($"Weapon_{data.weaponName}({starLevel}星)");
        weaponObj.transform.SetParent(transform);

        WeaponInstance instance = null;
        switch (data.weaponType)
        {
            //case EWeaponType.Melee:
            //    instance =  weaponObj.AddComponent<MeleeWeapon>();
            //    break;
            case EWeaponType.Ranged:
                instance =  weaponObj.AddComponent<RangedWeapon>();
                break;
            //case EWeaponType.Magic:
            //    instance =  weaponObj.AddComponent<MagicWeapon>();
            //    break;  
            default:
                instance =  weaponObj.AddComponent<WeaponInstance>();
                break;
        };
        instance.Initialize(data, weaponAttackPoint,starLevel);

        return instance;
    }
}

[System.Serializable]
public class ActiveSynergy
{
    public SynergyData data;
    public int tier;
    public int count;
}