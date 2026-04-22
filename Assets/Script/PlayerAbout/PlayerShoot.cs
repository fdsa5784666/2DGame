using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private Transform firePoint;
    private float timer = 0;
    private PlayerAutoLock autoLock;
    [SerializeField]
    private BulletInfo bulletInfo;

    private void Awake()
    {

        firePoint = transform;
        autoLock = GetComponent<PlayerAutoLock>();
        bulletInfo = new BulletInfo
        {
            // 战斗属性
            damage = 50f,
            speed = 5f,
            size = 1f,
            lifeTime = 10f,

            // 特殊能力
            pierceCount = 1,
            homing = false,
            homingStrength = 0f,
            splitCount = 0,
            splitAngle = 0f,
            explosionRadius = 0f,
            explosionDamageMultiplier = 1f,

            // 视觉效果
            bulletColor = Color.white,
            hitEffectPrefab = null,
            trailEffectPrefab = null,

            // 元数据
            isCritical = false
        };
    }
    void Update()
    {
        if (GameData.Instance.isLockEnemy && timer <= 0 && autoLock.CurrentTarget != null)
        {


            Vector2 direction = (autoLock.CurrentTarget.position - firePoint.position).normalized;

            //创建的时候顺便获取到新创建的对象 并使用其内部的位移相关函数进行设置
            GameObject bullet = ObjectPool.Instance.SpawnFromPool("Bullet", firePoint.position);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Initialize(bulletInfo);
            bulletScript.SetDirection(direction);

            timer = GameData.Instance.shootInterval * GameData.Instance.shootScale;
        }
        //以下为鼠标点击操控
        //if(Input.GetMouseButton(0) && timer <= 0)
        //{
        //    //屏幕坐标转世界坐标 用于计算子弹飞行方向
        //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    mousePos.z = 0;
        //    //单位化鼠标位置到角色的向量 用于表示方向 bullet的speed参数控制具体速度
        //    Vector2 direction = (mousePos - firePoint.position).normalized;

        //    //创建的时候顺便获取到新创建的对象 并使用其内部的位移相关函数进行设置
        //    GameObject bullet = ObjectPool.instance.SpawnFromPool("Bullet", firePoint.position, 0f);
        //    Bullet bulletScript = bullet.GetComponent<Bullet>();
        //    bulletScript.SetDirection(direction);

        //    timer = GameData.Instance.shootInterval * GameData.Instance.shootScale;
        //}
        timer -= Time.deltaTime;
    }
}

