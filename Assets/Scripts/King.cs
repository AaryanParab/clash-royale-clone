using UnityEngine;

public class King : MonoBehaviour
{
    [Header("King Attack Settings")]
    public float attackRange = 8f;           // Shooting range
    public float fireRate = 1.8f;            // Shots per second (slower and stronger than archers)
    public GameObject arrowPrefab;
    public Transform firePoint;              // Empty child at the top of the tower

    [Header("Target Settings")]
    public string enemyTroopTag = "EnemyTroop";   // Change this in Inspector for each king

    private float nextFireTime = 0f;
    private Transform currentTarget;

    private void Start()
    {
        if (firePoint == null)
            firePoint = transform;   // Fallback if no firePoint assigned
    }

    private void Update()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            FindNearestEnemyTroop();
        }

        if (currentTarget != null && Time.time >= nextFireTime)
        {
            ShootArrow();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    private void FindNearestEnemyTroop()
    {
        GameObject[] enemyTroops = GameObject.FindGameObjectsWithTag(enemyTroopTag);
        float shortestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject troop in enemyTroops)
        {
            if (!troop.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, troop.transform.position);
            if (distance <= attackRange && distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = troop.transform;
            }
        }

        currentTarget = nearest;
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null || currentTarget == null) return;

        // Spawn arrow from fire point
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up * 2f;
        GameObject arrowGO = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        ArrowProjectile projectile = arrowGO.GetComponent<ArrowProjectile>();
        if (projectile != null)
        {
            projectile.SetTarget(currentTarget, 0.8f);   // King deals 0.8 damage per arrow
        }

        // King faces the target while shooting
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        dir.y = 0f;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    // Visual range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}