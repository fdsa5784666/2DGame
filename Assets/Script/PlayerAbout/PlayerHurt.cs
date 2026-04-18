using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerHurt : MonoBehaviour,IDamageable
{
    private Transform playerTransform;
    private Vector3 playerRepeledPos;
    private Vector3 repelDrection;

    private float timer = 0f;
    private float hurtInterval = 1f;

    //受伤时闪屏变红的参数
    public Image damageFlashImage;
    private float flashDuration = 0.15f;
    private float flashAlpha = 0.4f;
    private Coroutine flashCoroutine;

    public GameObject HealthBar;
    private Transform HealthView;
    private Transform Text;
    private Image HealthView_UI;
    private TextMeshProUGUI Text_UI;

    /// <summary>
    /// 1. 初始化血量为maxHealth
    /// 2. 获取角色位置 用于击退效果
    /// 3. 初始化血条UI
    /// </summary>
    private void Start()
    {
        SetFlashAlpha(0f);
        GameData.Instance.playerCurrentHealth = (float)GameData.Instance.playerMaxHealth;

        playerTransform = transform;

        if ( HealthBar != null)
        {
            HealthView = HealthBar.transform.Find("HealthBar_Background/HealthView");
            Text = HealthBar.transform.Find("HealthBar_Background/Text");
            if( HealthView != null && Text != null )
            {
                HealthView_UI = HealthView.GetComponent<Image>();
                Text_UI = Text.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        GameData.Instance.playerCurrentHealth -= damage;
        //开始协程 执行闪屏特效
        if(damageFlashImage != null )
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashCoroutine());
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        //以下为受伤具体效果
        if ( collision.gameObject.CompareTag("Enemy") && timer <= 0 )
        {
            
            TakeDamage(GameData.Instance.enemyCollideDamage);

            Vector3 PlayerPos = playerTransform.position;
            repelDrection = ( PlayerPos - collision.transform.position ).normalized;
            playerTransform.position += repelDrection * GameData.Instance.repelMultValue;

            timer = hurtInterval;
        }
    }

    /// <summary>
    /// 血条刷新
    /// </summary>
    private void Update()
    {
        if (GameData.Instance != null && HealthView_UI != null && Text_UI != null && GameData.Instance.playerCurrentHealth <= 999999)
        {
            HealthView_UI.fillAmount = GameData.Instance.playerCurrentHealth / GameData.Instance.playerMaxHealth;
            Text_UI.text = "Hp " + GameData.Instance.playerCurrentHealth.ToString() + "/" + GameData.Instance.playerMaxHealth.ToString();
        }
        else if (GameData.Instance.playerCurrentHealth > 999999)
            Text_UI.text = "Hp beyond 1M";

        timer -= Time.deltaTime;
    }

    public IEnumerator FlashCoroutine()
    {
        //1.显示红色
        SetFlashAlpha(flashAlpha);
        //2.等0.4s
        yield return new WaitForSeconds(flashDuration);
        //3.渐变成透明
        float fadeOutTime = 0.1f;
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(flashAlpha, 0f, timer / fadeOutTime);
            SetFlashAlpha(alpha);
            yield return null;
        
        }
        //确保归零
        SetFlashAlpha (0f);
    }

    void SetFlashAlpha(float alpha)
    {
        Color color = damageFlashImage.color;
        color.a = alpha;    
        damageFlashImage.color = color;
    }
}
