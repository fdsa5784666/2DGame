using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooledable
{
    void OnObjectSpawn();
    void ReturnToPool();
    
}
