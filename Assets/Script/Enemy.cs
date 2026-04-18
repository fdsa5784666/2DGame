using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour,IDamageable,IDropItem
{
    [SerializeField]protected float maxHealth;
    [SerializeField]protected float currentHealth;
    [SerializeField]protected string id = "Enemy";
    public static List<Enemy> ActiveEnemies = new List<Enemy>();

    protected Transform moveTarget;
    /// <summary>
    /// 默认初始血量是默认最大血量
    /// 并 初始化 找到玩家的坐标引
    /// </summary>
    protected virtual void Awake()
    {
        if(moveTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if( player != null ) moveTarget = player.transform;
        }
    }

    protected virtual void Update()
    {
        EnemyMove();
    }
    protected virtual void EnemyMove()
    {
        
    }
    private void OnEnable()
    {
        ActiveEnemies.Add(this);
    }
    private void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }
    private void OnDestroy()
    {
        ActiveEnemies.Remove(this);
    }
    //抽象函数 必须在子类重写
    protected abstract void Die();
    public abstract void TakeDamage(float damage);
    public virtual void DropItem()
    {
        //随机掉落物品
        if (Random.value <= GameData.Instance.HealthPotionRandomValue)
        {
            Debug.Log($"{id}掉落了血瓶,快捡起来吧");
            Instantiate(GameData.Instance.HealthPotionPrefab, transform.position, Quaternion.identity);
        }
    }
}
