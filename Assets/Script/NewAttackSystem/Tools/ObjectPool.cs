using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    /// <summary>
    /// 对象池类
    /// 内含参数有 
    /// tag - 标志池子存放的对象类型
    /// prefab - 池子对应存放的对象预制体
    /// size - 初始池子大小
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    //存放各类池
    public List<Pool> pools;
    //管理各类池
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    //记录激活对象
    Dictionary<string,HashSet<GameObject>> activeByTag = new Dictionary<string,HashSet<GameObject>>();


    //初始化池子和池子字典信息
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>> ();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject> ();
            for(int i =0;i<pool.size;i++)
            {
                GameObject obj = CreateNewObject(pool.prefab);
                objectPool.Enqueue (obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
            activeByTag[pool.tag] = new HashSet<GameObject>();
        }
    }
    /// <summary>
    /// 在池中创建新对象
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    private GameObject CreateNewObject( GameObject prefab )
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        
        Vector3 pos = obj.transform.position;
        obj.transform.position = new Vector3(pos.x, pos.y, 0);

        return obj;
    }
    /// <summary>
    /// 从池子取出对象 
    /// 池子都在一起 所有需要池子对象可直接调用获取
    /// </summary>
    public GameObject SpawnFromPool(string tag,Vector2 position,float angleDegrees = 0f)
    {
        //寻找是否有对应类型对象池
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"对象池{tag}不存在");
            return null;
        }
        //取出对象池
        Queue<GameObject> pool = poolDictionary[tag];
        //判断池子是否还有对象 判空则扩容一个 给此次调用使用
        if(pool.Count == 0)
        {
            GameObject prefab = pools.Find(p => p.tag == tag).prefab;
            GameObject newObj = CreateNewObject(prefab);
            pool.Enqueue(newObj);
        }
        //从此开始取对象逻辑
        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.transform.position = new Vector3(position.x, position.y, 0);
        objectToSpawn.transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
        objectToSpawn.SetActive(true);

        if (!activeByTag.ContainsKey(tag))
            activeByTag[tag] = new HashSet<GameObject>();
        activeByTag[tag].Add(objectToSpawn);

        IPooledable pooledObject = objectToSpawn.GetComponent<IPooledable>();
        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }


        return objectToSpawn;


     }
    /// <summary>
    /// 第二个参数可传Vector3的重载
    /// </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, float angleDegrees = 0f)
    {
        Vector2 V2Pos = new Vector2(position.x, position.y);
        return SpawnFromPool(tag, V2Pos, angleDegrees);
    }
    /// <summary>
    /// 第二个参数可传transform的重载
    /// </summary>
    public GameObject SpawnFromPool(string tag, Transform T_position, float angleDegrees = 0f)
    {
        Vector2 V2Pos = new Vector2(T_position.position.x,T_position.position.y);
        return SpawnFromPool(tag, V2Pos, angleDegrees);
    }

    /// <summary>
    /// 还回池子逻辑
    /// </summary>
    public void ReturnToPool(string tag,GameObject obj)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Destroy(obj);
            return;
        }

        //IPooledable pooledObject = obj.GetComponent<IPooledable>();
        //if (pooledObject != null)
        //{
        //    pooledObject.ReturnToPool();
        //}

        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        poolDictionary[tag].Enqueue(obj);
        if(activeByTag.TryGetValue(tag,out var set))
            set.Remove(obj);
    }
    public void ReturnAll()
    {
        var allTags = new List<string>(activeByTag.Keys);
        Debug.Log("返还全部对象池物品");
        foreach (var tag in allTags)
        {
            var activeObj = new List<GameObject>();
            foreach (var obj in activeByTag[tag])
            {
                if(obj != null)
                    activeObj.Add(obj);
            }
            foreach(var obj in activeObj)
            {
                ReturnToPool(tag, obj);
            }

        }
        activeByTag.Clear();
    }
}
