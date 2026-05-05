using UnityEngine;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance { get; private set; }

    [Header("=== 引用 ===")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private WeaponSlotManager slotManager;

    [Header("=== 攻击配置 ===")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private bool autoAttack = true;

    //// 武器控制器映射
    //private Dictionary<WeaponInstance, WeaponController> controllerMap = new();
    //private List<WeaponController> activeControllers = new();

    // 缓存
    //private Transform nearestEnemy;
    //private float nearestEnemyUpdateTimer = 0f;
    //private const float NEAREST_ENEMY_UPDATE_INTERVAL = 0.1f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (slotManager == null)
            slotManager = WeaponSlotManager.Instance;

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (slotManager != null)
        {
            //slotManager.OnWeaponAdded += OnWeaponAdded;
            //slotManager.OnWeaponUpgraded += OnWeaponUpgraded;
            //slotManager.OnWeaponRemoved += OnWeaponRemoved;

            //foreach (var weapon in slotManager.EquippedWeapons)
            //{
            //    OnWeaponAdded(weapon);
            //}
        }
    }

    void Update()
    {
        if (!autoAttack) return;
        if (GameData.Instance != null && GameData.Instance.CurrentState != EGameState.Playing) return;

        //nearestEnemyUpdateTimer -= Time.deltaTime;
        //if (nearestEnemyUpdateTimer <= 0)
        //{
        //    nearestEnemy = FindNearestEnemy();
        //    nearestEnemyUpdateTimer = NEAREST_ENEMY_UPDATE_INTERVAL;
        //}

        //float deltaTime = Time.deltaTime;
        //foreach (var controller in activeControllers)
        //{
        //    controller.Update(deltaTime, nearestEnemy);
        //}
    }

    //void OnDestroy()
    //{
    //    if (slotManager != null)
    //    {
    //        slotManager.OnWeaponAdded -= OnWeaponAdded;
    //        slotManager.OnWeaponUpgraded -= OnWeaponUpgraded;
    //        slotManager.OnWeaponRemoved -= OnWeaponRemoved;
    //    }
    //}

    //#region 武器事件处理
    //private void OnWeaponAdded(WeaponInstance weapon)
    //{
    //    var controller = CreateController(weapon);
    //    if (controller != null)
    //    {
    //        controllerMap[weapon] = controller;
    //        activeControllers.Add(controller);
    //        controller.Initialize(weapon, playerTransform, enemyLayer);
    //    }
    //}

    //private void OnWeaponUpgraded(WeaponInstance weapon, int newLevel)
    //{
    //    if (controllerMap.TryGetValue(weapon, out var controller))
    //    {
    //        controller.OnUpgraded(newLevel);
    //    }
    //}

    //private void OnWeaponRemoved(WeaponInstance weapon)
    //{
    //    if (controllerMap.TryGetValue(weapon, out var controller))
    //    {
    //        controller.Cleanup();
    //        activeControllers.Remove(controller);
    //        controllerMap.Remove(weapon);
    //    }
    //}
    //#endregion

    //#region 控制器创建
    //private WeaponController CreateController(WeaponInstance weapon)
    //{
    //    return weapon.Data.weaponType switch
    //    {
    //        EWeaponType.Ranged => new RangedWeaponController(),
    //        //EWeaponType.Melee => new MeleeWeaponController(),
    //        //EWeaponType.Laser => new LaserWeaponController(),
    //        //EWeaponType.AoE => new AoEWeaponController(),
    //        //EWeaponType.Summon => new SummonWeaponController(),
    //        _ => new RangedWeaponController()
    //    };
    //}
    //#endregion

    #region 敌人查找
    //private Transform FindNearestEnemy()
    //{
    //    if (playerTransform == null) return null;

    //    float searchRadius = 20f;
    //    var hits = Physics2D.OverlapCircleAll(playerTransform.position, searchRadius, enemyLayer);

    //    Transform nearest = null;
    //    float minDist = float.MaxValue;

    //    foreach (var hit in hits)
    //    {
    //        float dist = Vector2.Distance(playerTransform.position, hit.transform.position);
    //        if (dist < minDist)
    //        {
    //            minDist = dist;
    //            nearest = hit.transform;
    //        }
    //    }

    //    return nearest;
    //}

    //public Transform GetNearestEnemy() => nearestEnemy;

    //public List<Transform> GetAllEnemiesInRange(float range)
    //{
    //    if (playerTransform == null) return new List<Transform>();

    //    var hits = Physics2D.OverlapCircleAll(playerTransform.position, range, enemyLayer);
    //    var enemies = new List<Transform>();

    //    foreach (var hit in hits)
    //    {
    //        enemies.Add(hit.transform);
    //    }

    //    return enemies;
    //}
    #endregion

    #region 公共方法
    public void SetAutoAttack(bool enabled)
    {
        autoAttack = enabled;
    }
    public bool AutoAttack => autoAttack;

    public void ForceAttack()
    {
        //foreach (var controller in activeControllers)
        //{
        //    controller.ForceAttack(nearestEnemy);
        //}
    }

    //public WeaponController GetController(WeaponInstance weapon)
    //{
    //    controllerMap.TryGetValue(weapon, out var controller);
    //    return controller;
    //}
    #endregion
}