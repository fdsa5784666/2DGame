using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //检测是否是可拾取物品 
        //再检测是哪一个 最后删除该可拾取物品
        IPickable iPickable = collision.GetComponent<IPickable>();
        if(iPickable != null )
        {
            if( collision.CompareTag("HealthPotion"))
            {
                iPickable.PickItem();
            }
            Destroy(collision.gameObject);
        }
        
    }
}
