using UnityEngine;

public class ArcherAI : TroopAI
{
    public GameObject arrowPrefab;

    protected override void Awake()
    {
        base.Awake();
        stoppingDistance = 3.5f;      // Archer stops farther away
        attackRange = 4.0f;           // Shoots from this distance
        moveSpeed = 2.3f;
        damagePerSecond = 20f;
        attackCooldown = 1.3f;
    }

    protected override void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        if (arrowPrefab != null && currentTarget != null)
        {
            // Shoot arrow
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
            GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.LookRotation(currentTarget.position - transform.position));
            ArrowProjectile proj = arrow.GetComponent<ArrowProjectile>();
            if (proj != null)
                proj.SetTarget(currentTarget, 1f);   // 1 damage per arrow
        }
        else
        {
            // Fallback direct damage
            Tower tower = currentTarget.GetComponent<Tower>();
            if (tower != null)
                tower.TakeDamage(damagePerSecond * attackCooldown);
        }
    }
}