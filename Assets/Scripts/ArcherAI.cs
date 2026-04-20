using UnityEngine;

public class ArcherAI : TroopAI
{
    [Header("Archer Specific")]
    public GameObject arrowPrefab;
    public float arrowSpawnHeight = 1.4f;

    protected override void Awake()
    {
        base.Awake();
        
        stoppingDistance = 3.8f;
        attackRange = 4.5f;
        moveSpeed = 2.2f;
        damagePerSecond = 1f;
        attackCooldown = 1.3f;
    }
    
    protected override void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        if (arrowPrefab != null && currentTarget != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * arrowSpawnHeight;

            GameObject arrowGO = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

            ArrowProjectile projectile = arrowGO.GetComponent<ArrowProjectile>();
            if (projectile != null)
            {
                projectile.SetTarget(currentTarget, 1f);   // 1 damage per arrow
            }
            
            Vector3 dir = (currentTarget.position - transform.position).normalized;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        
        if (animator != null)
            Invoke("TriggerAttackAnimation", attackAnimationDelay);
    }
}