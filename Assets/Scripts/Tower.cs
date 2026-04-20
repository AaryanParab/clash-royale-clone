using UnityEngine;
using UnityEngine.Events;

public class Tower : MonoBehaviour
{
    [Header("Tower Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public HealthBarUI healthBar;

    [Header("Events")]
    public UnityEvent onDeath;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
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
        
        if (UIManager.Instance != null)
        {
            string message = gameObject.name.Contains("Player") ? "Enemy Wins!" : "Player Wins!";
            UIManager.Instance.ShowEndScreen(message);
        }

        onDeath?.Invoke();
        
        Destroy(gameObject, 0.5f);
    }
    
    [ContextMenu("Take 20 Damage")]
    private void TestDamage()
    {
        TakeDamage(20f);
    }
}