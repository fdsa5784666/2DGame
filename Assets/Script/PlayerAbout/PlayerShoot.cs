using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标左键点击触发的子弹发射
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    private Transform firePoint;
    private float timer = 0;
    private PlayerAutoLock autoLock;

    private void Awake()
    {
        firePoint = transform;
        autoLock = GetComponent<PlayerAutoLock>();
    }
    void Update()
    {
        if (GameData.Instance.isLockEnemy && timer <= 0 && autoLock.GetCurrentTarget() != null)
        {


            Vector2 direction = (autoLock.GetCurrentTarget().position - firePoint.position).normalized;

            //创建的时候顺便获取到新创建的对象 并使用其内部的位移相关函数进行设置
            GameObject bullet = ObjectPool.instance.SpawnFromPool("Bullet", firePoint.position, 0f);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetDirection(direction);

            timer = GameData.Instance.shootInterval * GameData.Instance.shootScale;
        }
        //以下为鼠标电机操控
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

