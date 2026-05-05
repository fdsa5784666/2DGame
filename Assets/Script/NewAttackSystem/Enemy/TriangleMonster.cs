using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriangleMonster : Enemy,IPooledable
{
    private Transform ModelGO;
    private SpriteRenderer Model;
    public GameObject deathEffectPrefab;

    // 原始数据缓存（用于重置）
    private float originalMaxHealth;
    private float originalMoveSpeed;


    protected override void Awake()
    {
        base.Awake();

        originalMaxHealth = maxHealth;
        ModelGO = transform.Find("model");
        Model = ModelGO.gameObject.GetComponent<SpriteRenderer>();

    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        //父类有调用EnemyMove
        base.Update();
    }

    protected override void Die()
    {
        isAlive = false;

        DropItem();
        GameData.Instance.AddGold(GameData.Instance.TriangleMonsterGold);
        GameData.Instance.AddKill() ;

        GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = deathEffect.GetComponent<ParticleSystem>();
         if(ps != null)
        {
            ps.Play();
            Destroy(deathEffect, ps.main.duration);
        }
        else
        {
            Destroy(deathEffect, 2f);
        }

         EnemySpawner.Instance.OnEnemyKilled(this.gameObject);
        ReturnToPool();
    }

    /// <summary>
    /// 此敌方单位追击逻辑是直接贴近
    /// 已继承 父类找到的玩家位置 moveTarget
    /// 并且 转向玩家
    /// </summary>
    protected override void EnemyMove()
    {

        Vector3 currentPlayerPos = moveTarget.position;
        Vector3 direction = (currentPlayerPos - transform.position).normalized;
        transform.position += direction * GameData.Instance.TriangleMoveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    public override void DropItem()
    {
        base.DropItem();
        //此处实现掉落物品功能 (经验)
        GameObject EXP = ObjectPool.Instance.SpawnFromPool("EXP", transform.position, 0f);
        EXP newEXP = EXP.GetComponent<EXP>();
        newEXP.EXPValue = GameData.Instance.TriangleMonsterEXP;

        
    }
    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        isAlive = true;
        Model.color = new Color(238f / 255f, 78f / 255f, 78f / 255f);
    }
    public void ReturnToPool()
    {
        ObjectPool.Instance.ReturnToPool("TriangleMonster", gameObject);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        ResetEnemy();
    }
    public void OnObjectSpawn()
    {
        id = "TriangleMonster";
        maxHealth = GameData.Instance.EnemyMaxHp;
        currentHealth = maxHealth;
    }
    public void TakeDamageAnimate()
    {
        Model.color = new Color(238f/255f,78f/255f,78f/255f);
    }
    public override void TakeDamage(float damage, bool isCritical = false)
    {
        base.TakeDamage(damage, isCritical);
        Model.color = Color.white;
        Invoke(nameof(TakeDamageAnimate), 0.2f);
    }
}
