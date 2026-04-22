using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, bool isCritical = false);
    Transform GetTransform();
    bool IsAlive();
    ETeam GetTeam();
}

public enum ETeam
{
    Player,
    Enemy,
    Neutral
}