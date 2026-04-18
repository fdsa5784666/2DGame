using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour, IPooledable
{
    //子弹及时不离开场景或者产生碰撞 十秒也自然消亡
    //否则同时活跃对象容易过多
    private float offscreenPadding = 10f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if( IsOffScreen() )
        {
            ReturnToPool();
        }
        
    }

    private bool IsOffScreen()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        if(screenPos.x < -offscreenPadding||
           screenPos.x > Screen.width + offscreenPadding||
           screenPos.y < -offscreenPadding||
           screenPos.y > Screen.height + offscreenPadding)
        {
            return true;
        }
        return false;
    }
    public void OnObjectSpawn()
    {
        CancelInvoke();
        Invoke(nameof(ReturnToPool), GameData.Instance.bulletLifeTime);
    }
    public void ReturnToPool()
    {
        rb.velocity = Vector2.zero;
        ObjectPool.instance.ReturnToPool("Bullte", gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {

            //以下是在检测敌人是否可受伤
            if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(GameData.Instance.bulletDamage);
                Debug.Log($"造成了{GameData.Instance.bulletDamage}点伤害");
                if (!GameData.Instance.bulletPenetrable)
                {
                    ReturnToPool();
                }
            }
            ReturnToPool();

        }
        else if(other.CompareTag("Wall"))
        {
            ReturnToPool();
        }
        
    }
    public void SetDirection(Vector2 diretion)
    {
        moveDirection = diretion.normalized;
        rb.velocity = moveDirection * GameData.Instance.bulletSpeed;
    }
}
