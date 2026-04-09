using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Arrow Settings")]
    public float speed = 20f;
    public float lifetime = 5f;           // Destroy after this time if it misses
    public float damage = 10f;

    private Transform target;
    private bool hasHit = false;

    public void SetTarget(Transform targetTransform, float damageAmount)
    {
        target = targetTransform;
        damage = damageAmount;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);   // Auto destroy if it doesn't hit anything
    }

    private void Update()
    {
        if (hasHit || target == null) return;

        // Move towards the target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Face the direction of movement
        transform.rotation = Quaternion.LookRotation(direction);

        // Check if close enough to hit
        if (Vector3.Distance(transform.position, target.position) < 0.8f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        hasHit = true;

        // Deal damage to the tower
        Tower tower = target.GetComponent<Tower>();
        if (tower != null)
        {
            tower.TakeDamage(damage);
        }

        // Optional: Add hit effect here (particles, sound, etc.)
        // Instantiate(hitEffect, transform.position, Quaternion.identity);

        // Destroy the arrow
        Destroy(gameObject, 0.1f);
    }

    // Optional: Destroy on collision with anything (safety)
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        Tower tower = other.GetComponent<Tower>();
        if (tower != null)
        {
            HitTarget();
        }
    }
}