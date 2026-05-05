using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour,IDamageable,IDropItem
{
    [Header("=== 基础属性 ===")]
    [SerializeField]protected float maxHealth;
    [SerializeField]protected float currentHealth;
    [SerializeField]protected string id = "Enemy";
    [SerializeField]protected ETeam team = ETeam.Enemy;
   

    [Header("=== 受击反馈 ===")]
    [SerializeField] protected float hitFlashDuration = 0.1f;
    [SerializeField] protected Color hitFlashColor = new Color(238f / 255f, 78f / 255f, 78f / 255f);

    public static List<Enemy> ActiveEnemies = new List<Enemy>();
    public event System.Action<Enemy> OnDeath;
    public GameObject OriginalPrefab { get; set; }

    protected Transform moveTarget;
    //protected SpriteRenderer modelRenderer;
    //protected Color originalColor;
    protected bool isAlive = true;

    // 运行时倍率（用于难度调整）
    protected float healthMultiplier = 1f;
    protected float speedMultiplier = 1f;
    protected float damageMultiplier = 1f;
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

        // 尝试获取模型上的 SpriteRenderer
        //Transform modelTrans = transform.Find("model");
        //if (modelTrans != null)
        //{
        //    modelRenderer = modelTrans.GetComponent<SpriteRenderer>();
        //    if (modelRenderer != null)
        //    {
        //        originalColor = modelRenderer.color;
        //    }
        //}
    }
    protected virtual void Start()
    {
        currentHealth = maxHealth * healthMultiplier;
        isAlive = true;
    }
    protected virtual void Update()
    {
        if (!isAlive) return;
        if (GameData.Instance != null && GameData.Instance.CurrentState != EGameState.Playing) return;
        EnemyMove();
    }
    protected virtual void EnemyMove()
    {
        
    }
    protected virtual void OnEnable()
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

    #region IDamageable 实现
    public virtual void TakeDamage(float damage, bool isCritical = false)
    {
        if (!isAlive) return;
        float fDamage = damage * GameData.Instance.FinalCriticalDamage;
        if (isCritical)
        {
            currentHealth -= fDamage;
            Debug.Log($"{id}受到暴击伤害: {fDamage}, 当前血量: {currentHealth}/{maxHealth * healthMultiplier}");
        }
        else
        {
            Debug.Log($"常规{damage}伤害");
            currentHealth -= damage;
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public ETeam GetTeam()
    {
        return team;
    }
    #endregion

    #region 死亡与掉落
    protected virtual void Die()
    {
        OnDeath?.Invoke(this);
    }

    public virtual void DropItem()
    {
        // 随机掉落血瓶
        if (Random.value <= GameData.Instance.HealthPotionRandomValue)
        {
            Debug.Log($"{id}掉落了血瓶");
            Instantiate(GameData.Instance.HealthPotionPrefab, transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region 难度调整
    /// <summary>
    /// 设置难度倍率（由 EnemySpawner 调用）
    /// </summary>
    public virtual void SetMultipliers(float healthMult, float damageMult, float speedMult)
    {
        // 按比例调整当前血量
        float healthPercent = currentHealth / (maxHealth * healthMultiplier);
        healthMultiplier = healthMult;
        damageMultiplier = damageMult;
        speedMultiplier = speedMult;

        maxHealth = maxHealth * healthMultiplier;
        currentHealth = maxHealth * healthPercent;
    }
    #endregion
}
