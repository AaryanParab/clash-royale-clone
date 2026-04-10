using UnityEngine;

public class KnightAI : TroopAI
{
    protected override void Awake()
    {
        base.Awake();

        stoppingDistance = 0.5f;     // Gets nicely onto the small green platform
        attackRange = 1f;          // Starts attacking when close enough to visually hit
        moveSpeed = 0.3f;
        damagePerSecond = 0.5f;
        attackCooldown = 1.0f;
    }
}