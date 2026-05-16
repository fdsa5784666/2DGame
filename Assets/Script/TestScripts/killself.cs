using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killself : MonoBehaviour
{
    public PlayerHurt p;
    public void OnClick()
    {
        p.TakeDamage(9999);
    }

}
