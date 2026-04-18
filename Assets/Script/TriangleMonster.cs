using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriangleMonster : Enemy, IDamageable,IPooledable
{
    private Transform ModelGO;
    private SpriteRenderer Model;
    public GameObject deathEffectPrefab;
    protected override void Awake()
    {
        base.Awake();
        ModelGO = transform.Find("model");
        Model = ModelGO.gameObject.GetComponent<SpriteRenderer>();

    }
    protected override void Update()
    {
        //父类有调用EnemyMove
        base.Update();
    }

    protected override void Die()
    {
        DropItem();
        GameData.Instance.killedEnemy++;

        GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = deathEffect.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(deathEffect,ps.main.duration);


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
        transform.position += direction * GameData.Instance.triangleMoveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        if (Model != null)
        {
            Model.color = Color.white;
            Invoke("TakeDamageAnimate", 0.2f);
        }

    }
    public override void DropItem()
    {
        base.DropItem();
        //此处实现掉落物品功能 (经验)
        GameObject EXP = ObjectPool.instance.SpawnFromPool("EXP", transform.position, 0f);
        EXP newEXP = EXP.GetComponent<EXP>();
        newEXP.EXPValue = GameData.Instance.triangleMonsterEXP;

        
    }

    public void ReturnToPool()
    {
        Debug.Log("enemy对象已回收");
        ObjectPool.instance.ReturnToPool("TriangleMonster", gameObject);
    }

    public void OnObjectSpawn()
    {
        id = "TriangleMonster";
        maxHealth = GameData.Instance.enemyMaxHp;
        currentHealth = maxHealth;
    }
    public void TakeDamageAnimate()
    {
        Model.color = new Color(238f/255f,78f/255f,78f/255f);
    }

}
