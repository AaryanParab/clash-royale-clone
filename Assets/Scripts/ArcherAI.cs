using UnityEngine;

public class ArcherAI : TroopAI
{
    [Header("Archer Specific")]
    public GameObject arrowPrefab;
    public float arrowSpawnHeight = 1.4f;

    protected override void Awake()
    {
        base.Awake();

        // Archer settings
        stoppingDistance = 3.8f;      // Stop at shooting distance
        attackRange = 4.5f;           // Shoot from this range
        moveSpeed = 2.2f;
        damagePerSecond = 1f;         // Not used for archer, but kept for consistency
        attackCooldown = 1.3f;
    }

    // Override AttackTarget to shoot arrow instead of direct damage
    protected override void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        if (arrowPrefab != null && currentTarget != null)
        {
            // Spawn arrow slightly above the archer
            Vector3 spawnPos = transform.position + Vector3.up * arrowSpawnHeight;

            GameObject arrowGO = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

            ArrowProjectile projectile = arrowGO.GetComponent<ArrowProjectile>();
            if (projectile != null)
            {
                projectile.SetTarget(currentTarget, 1f);   // 1 damage per arrow
            }

            // Make sure archer faces the target while shooting
            Vector3 dir = (currentTarget.position - transform.position).normalized;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Trigger attack animation
        if (animator != null)
            Invoke("TriggerAttackAnimation", attackAnimationDelay);
    }
}