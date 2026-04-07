using UnityEngine;
using UnityEngine.Events;

public class Tower : MonoBehaviour
{
    [Header("Tower Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Visuals")]
    //public HealthBarUI healthBar;           // Assign your HP bar if you have one

    [Header("Events")]
    public UnityEvent onDeath;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    /// <summary>
    /// Damages the tower
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        //if (healthBar != null)
        {
            //healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " has been destroyed!");

        // Optional death effect
        // if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);

        onDeath?.Invoke();

        // You can destroy or disable the tower here
        // Destroy(gameObject, 1.5f);
    }

    // Optional: Heal function
    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }
}