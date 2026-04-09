using UnityEngine;

public class KnightAI : TroopAI
{
    protected override void Awake()
    {
        base.Awake();

        stoppingDistance = 1.4f;     // Gets nicely onto the small green platform
        attackRange = 1.7f;          // Starts attacking when close enough to visually hit
        moveSpeed = 2.6f;
        damagePerSecond = 30f;
        attackCooldown = 1.0f;
    }
}