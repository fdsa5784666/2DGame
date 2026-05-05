using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledable
{
    //[Header("组件")]
    //[SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private TrailRenderer trailRenderer;

    private BulletInfo info;
    private int remainingPierce;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private Transform homingTarget;
    private float offscreenPadding = 10f;
    private Rigidbody2D rb;
    private Camera mainCamera;

    // 对象池标签
    private const string POOL_TAG = "Bullet";

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        //if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 根据传入的BulletInfo初始化子弹属性和外观
    /// </summary>
    public void Initialize(BulletInfo bulletInfo)
    {
        info = bulletInfo;
        remainingPierce = info.pierceCount;

        // 重置状态
        hitEnemies.Clear();
        homingTarget = null;

        // 应用外观
        //transform.localScale = Vector3.one * info.size;
        //if (spriteRenderer != null)
        //{
        //    spriteRenderer.color = info.bulletColor;
        //}

        // 拖尾特效
        //if (info.trailEffectPrefab != null)
        //{
        //    if (trailRenderer != null)
        //    {
        //        // 复用已有的 TrailRenderer
        //        trailRenderer.Clear();
        //    }
        //    else
        //    {
        //        GameObject trail = Instantiate(info.trailEffectPrefab, transform);
        //        trailRenderer = trail.GetComponent<TrailRenderer>();
        //    }
        //}

        // 追踪
        //if (info.homing)
        //{
        //    StartCoroutine(HomingRoutine());
        //}
        // 生命周期
        Invoke(nameof(Return), info.lifeTime);
    }

    void Return()
    {
        ReturnToPool();
    }


    private void Update()
    {
        // 如果有 Rigidbody2D，用物理移动；否则用 Transform
        if (rb != null && rb.bodyType != RigidbodyType2D.Static)
        {
            rb.velocity = transform.right * info.speed;
        }
        else
        {
            transform.Translate(Vector3.right * info.speed * Time.deltaTime);
        }

        // 屏幕外检测
        if (IsOffScreen())
        {
            Debug.Log("Bullet Off Screen");
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitEnemies.Contains(other.gameObject)) return;

        // 使用接口造成伤害
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            if (!damageable.IsAlive()) return;

            damageable.TakeDamage(info.damage, info.isCritical);
            hitEnemies.Add(other.gameObject);

            // 命中特效
            //if (info.hitEffectPrefab != null)
            //{
            //    Instantiate(info.hitEffectPrefab, transform.position, Quaternion.identity);
            //}

            // 爆炸
            //if (info.explosionRadius > 0)
            //{
            //    Explode();
            //    ReturnToPool();
            //    return;
            //}

            // 分裂
            //if (info.splitCount > 0)
            //{
            //    Split();
            //}

            // 穿透
            remainingPierce--;
            if (remainingPierce <= 0)
            {
                ReturnToPool();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("撞墙回收");
            ReturnToPool();
        }
    }

    //private void Explode()
    //{
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, info.explosionRadius);

    //    foreach (Collider2D hit in hits)
    //    {
    //        if (!hit.CompareTag("Enemy")) continue;
    //        if (hitEnemies.Contains(hit.gameObject)) continue;

    //        if (hit.TryGetComponent<IDamageable>(out IDamageable damageable))
    //        {
    //            if (!damageable.IsAlive()) continue;

    //            // 距离衰减
    //            float distance = Vector2.Distance(transform.position, hit.transform.position);
    //            float falloff = 1 - (distance / info.explosionRadius);
    //            float damage = info.damage * info.explosionDamageMultiplier * Mathf.Max(0.3f, falloff);

    //            damageable.TakeDamage(damage, false);
    //            hitEnemies.Add(hit.gameObject);
    //        }
    //    }

    //    // 爆炸特效（可选）
    //    // if (explosionEffectPrefab != null)
    //    //     Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    //}

    //private void Split()
    //{
    //    for (int i = 0; i < info.splitCount; i++)
    //    {
    //        float angle = -info.splitAngle / 2 + (info.splitAngle / (info.splitCount - 1)) * i;
    //        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, angle);

    //        // 从对象池获取分裂子弹
    //        GameObject splitObj = ObjectPool.Instance?.SpawnFromPool(POOL_TAG, transform.position, angle);

    //        if (splitObj == null)
    //        {
    //            splitObj = Instantiate(gameObject, transform.position, rotation);
    //        }

    //        Bullet splitBullet = splitObj.GetComponent<Bullet>();
    //        if (splitBullet != null)
    //        {
    //            // 分裂子弹继承属性，但伤害减半
    //            BulletInfo splitInfo = info;
    //            splitInfo.damage *= 0.5f;
    //            splitInfo.splitCount = 0;  // 防止无限分裂
    //            splitBullet.Initialize(splitInfo);
    //        }
    //    }
    //}

    //private IEnumerator HomingRoutine()
    //{
    //    float updateInterval = 0.1f;
    //    float timer = 0f;

    //    while (true)
    //    {
    //        timer += Time.deltaTime;
    //        if (timer >= updateInterval)
    //        {
    //            timer = 0f;
    //            homingTarget = FindNearestEnemy();
    //        }

    //        if (homingTarget != null)
    //        {
    //            Vector2 direction = (homingTarget.position - transform.position).normalized;
    //            transform.right = Vector2.Lerp(transform.right, direction, info.homingStrength * Time.deltaTime);
    //        }

    //        yield return null;
    //    }
    //}

    //private Transform FindNearestEnemy()
    //{
    //    float searchRadius = 15f;
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius);

    //    Transform nearest = null;
    //    float minDist = Mathf.Infinity;

    //    foreach (Collider2D hit in hits)
    //    {
    //        if (hit.CompareTag("Enemy"))
    //        {
    //            float dist = Vector2.Distance(transform.position, hit.transform.position);
    //            if (dist < minDist)
    //            {
    //                minDist = dist;
    //                nearest = hit.transform;
    //            }
    //        }
    //    }
    //    return nearest;
    //}

    private bool IsOffScreen()
    {
        if (mainCamera == null) return false;

        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        // 使用 Viewport 坐标更简单
        float padding = 0.1f;
        return screenPos.x < -padding || screenPos.x > 1 + padding ||
               screenPos.y < -padding || screenPos.y > 1 + padding;
    }

    /// <summary>
    /// 设置子弹方向
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        transform.right = direction.normalized;
        if (rb != null)
        {
            rb.velocity = transform.right * info.speed;
        }
    }

    #region IPooledable 实现
    public void OnObjectSpawn()
    {
        // 取消之前的延迟调用
        CancelInvoke();
        // 清空命中记录
        hitEnemies.Clear();
        // 停止所有协程（防止追踪协程残留）
        StopAllCoroutines();
    }

    public void OnObjectReturn()
    {
        StopAllCoroutines();
        if (rb != null) rb.velocity = Vector2.zero;
        //if (trailRenderer != null) trailRenderer.Clear();
        hitEnemies.Clear();
        homingTarget = null;
        CancelInvoke();
    }

    public void ReturnToPool()
    {
        CancelInvoke();
        StopAllCoroutines();
        if (rb != null) rb.velocity = Vector2.zero;

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
}