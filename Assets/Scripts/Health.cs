using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Optional")]
    public bool destroyOnDeath = true;

    [Header("Events")]
    public UnityEvent onDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        onDeath?.Invoke();
        Debug.Log(gameObject.name + " has died!");

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            enabled = false;
            var ai = GetComponent<TroopAI>();
            if (ai != null) ai.enabled = false;
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetHealthPercentage() => maxHealth > 0 ? currentHealth / maxHealth : 0f;

    [ContextMenu("Take 1 Damage")]
    private void TestTakeDamage()
    {
        TakeDamage(1f);
    }
}