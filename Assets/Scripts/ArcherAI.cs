using UnityEngine;

public class ArcherAI : TroopAI
{
    [Header("Archer Specific")]
    public GameObject arrowPrefab;
    public Transform shootPoint;

    protected override void Awake()
    {
        base.Awake();
        moveSpeed = 3.8f;
        attackRange = 6.8f;
        damagePerSecond = 1.15f;
        attackCooldown = 0.9f;
    }

    protected override void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown) 
            return;

        lastAttackTime = Time.time;

        if (arrowPrefab && shootPoint && currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - shootPoint.position).normalized;
            GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.LookRotation(direction));

            //ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();
            //if (projectile != null)
            {
               // projectile.SetTarget(currentTarget, damagePerSecond * attackCooldown);
            }
        }
        else
        {
            base.AttackTarget();
        }

        // Animation commented out
        // if (animator) animator.SetTrigger("Attack");
    }
}