using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour ,IPickable, IPooledable
{
    public string itemName;

    public virtual void OnObjectSpawn()
    {
    }

    public virtual void PickItem()
    {
        
    }

    public virtual void ReturnToPool()
    {
    }
}
