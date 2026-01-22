using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// Manages player health and damage
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfigSO gameConfig;
    
    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float currentHealth;
    
    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged { get; private set; } = new(); // current, max
    public UnityEvent OnDamaged { get; private set; } = new();
    public UnityEvent OnDeath { get; private set; } = new();
    
    [Header("Visual Feedback")]
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;
    
    private bool isInvincible = false;
    private Color originalColor;
    private Material playerMaterial;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;
    
    private void Awake()
    {
        if (gameConfig != null)
        {
            maxHealth = gameConfig.StartingPlayerHealth;
        }
        
        currentHealth = maxHealth;
        
        // Get renderer and material
        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }
        
        if (playerRenderer != null)
        {
            playerMaterial = playerRenderer.material;
            originalColor = playerMaterial.color;
        }
    }
    
    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// Apply damage to the player
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (!IsAlive || isInvincible) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamaged?.Invoke();
        
        // Visual feedback
        FlashDamage();
        FeedbackManager.Instance.TriggerDamageFeedback();
        
        // Start invincibility frames
        if (gameConfig != null)
        {
            StartInvincibility(gameConfig.InvincibilityDuration);
        }
        else
        {
            StartInvincibility(1f);
        }
        
        // Check for death
        if (currentHealth <= 0f)
        {
            Die();
            FeedbackManager.Instance.TriggerDeathFeedback();
        }
    }
    
    /// <summary>
    /// Heal the player
    /// </summary>
    public void Heal(float amount)
    {
        if (!IsAlive) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        Debug.Log($"Player healed {amount}. Health: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// Start invincibility frames
    /// </summary>
    private void StartInvincibility(float duration)
    {
        isInvincible = true;
        
        // Flash the player during invincibility
        if (playerRenderer != null)
        {
            DOTween.Sequence()
                .Append(playerRenderer.material.DOFade(0.3f, 0.1f))
                .Append(playerRenderer.material.DOFade(1f, 0.1f))
                .SetLoops((int)(duration / 0.2f))
                .OnComplete(() => {
                    isInvincible = false;
                    if (playerMaterial != null)
                    {
                        playerMaterial.color = originalColor;
                    }
                });
        }
        else
        {
            // Fallback if no renderer
            Invoke(nameof(EndInvincibility), duration);
        }
    }
    
    private void EndInvincibility()
    {
        isInvincible = false;
    }
    
    /// <summary>
    /// Flash damage color
    /// </summary>
    private void FlashDamage()
    {
        if (playerMaterial == null) return;
        
        playerMaterial.DOColor(damageFlashColor, flashDuration)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => {
                if (playerMaterial != null)
                {
                    playerMaterial.color = originalColor;
                }
            });
    }
    
    /// <summary>
    /// Handle player death
    /// </summary>
    private void Die()
    {
        Debug.Log("Player died!");
        
        OnDeath?.Invoke();
        
        // Trigger game over
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.GameOver();
        }
        
        // Death animation
        if (playerRenderer != null)
        {
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Handle collision with enemies
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                TakeDamage(enemy.CollisionDamage);
            }
        }
        
        // Handle collision with enemy projectiles
        if (other.CompareTag("EnemyProjectile"))
        {
            EnemyProjectile projectile = other.GetComponent<EnemyProjectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.Damage);
                Destroy(other.gameObject);
            }
        }
    }
}
