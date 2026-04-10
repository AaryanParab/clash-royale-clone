using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 12f;
    public float damage = 1f;
    public float lifetime = 5f;

    private Transform target;
    private bool hasHit = false;

    public void SetTarget(Transform newTarget, float damageAmount)
    {
        target = newTarget;
        damage = damageAmount;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);   // Safety destroy
    }

    private void Update()
    {
        if (hasHit || target == null) 
        {
            Destroy(gameObject);
            return;
        }

        // Move toward target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate to face target
        transform.rotation = Quaternion.LookRotation(direction);

        // Check if close enough to hit
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (hasHit) return;
        hasHit = true;

        // Deal damage - works on BOTH troops (Health) and towers (Tower)
        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        else
        {
            Tower tower = target.GetComponent<Tower>();
            if (tower != null)
                tower.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    // Optional: Visual hit effect
    private void OnDestroy()
    {
        // You can instantiate a hit particle here later
    }
}