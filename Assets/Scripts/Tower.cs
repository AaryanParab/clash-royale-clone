using UnityEngine;
using UnityEngine.Events;

public class Tower : MonoBehaviour
{
    [Header("Tower Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public HealthBarUI healthBar;           // Assign your health bar UI here

    [Header("Events")]
    public UnityEvent onDeath;              // Called when tower is destroyed

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    /// <summary>
    /// Call this when any troop attacks the tower
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        UpdateHealthBar();

        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has been destroyed!");

        // Trigger win/lose condition in GameManager
        onDeath?.Invoke();

        // Optional: Play death effect or disable tower
        // gameObject.SetActive(false);
    }

    // Optional: For testing in Inspector
    [ContextMenu("Take 20 Damage")]
    private void TestDamage()
    {
        TakeDamage(20f);
    }
}