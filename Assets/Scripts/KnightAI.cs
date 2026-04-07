using UnityEngine;

public class KnightAI : TroopAI
{
    protected override void Awake()
    {
        base.Awake();
        moveSpeed = 2.3f;
        attackRange = 1.9f;
        damagePerSecond = 2.8f;
        attackCooldown = 1.35f;
    }
}