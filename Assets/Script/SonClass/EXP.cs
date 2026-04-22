using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXP : Item,IPickable_EXP
{
    [HideInInspector]
    public int EXPValue;

    public EXP()
    {
        itemName = "EXP";
    }

    public override void ReturnToPool()
    {
        ObjectPool.Instance.ReturnToPool("EXP", gameObject);
    }

    public int PickEXP()
    {
        return EXPValue;
    }
}
