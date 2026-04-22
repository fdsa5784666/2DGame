using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 回满血
/// </summary>
public class HealthPotion : Item
{

    public HealthPotion()
    {
        itemName = "HealthPotion";
    }

    public override void PickItem()
    {
        base.PickItem();
        GameData.Instance.FullHP();
    }
}
