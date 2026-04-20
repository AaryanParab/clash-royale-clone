using UnityEngine;

public class KnightAI : TroopAI
{
    protected override void Awake()
    {
        base.Awake();

        stoppingDistance = 0.5f;
        attackRange = 1f;
        moveSpeed = 0.3f;
        damagePerSecond = 0.5f;
        attackCooldown = 1.0f;
    }
}