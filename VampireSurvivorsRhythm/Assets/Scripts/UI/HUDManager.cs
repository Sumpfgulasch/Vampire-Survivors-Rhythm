using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the HUD display (health, experience, level)
/// </summary>
public class HUDManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Experience")]
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI experienceText;
    
    [Header("Level")]
    [SerializeField] private TextMeshProUGUI levelText;
    
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI attackCountText;
    
    private void Start()
    {
        // Subscribe to events
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelUp.AddListener(UpdateLevel);
            LevelManager.Instance.OnExperienceChanged.AddListener(UpdateExperience);
        }
        
        // Find player and subscribe to health events
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.AddListener(UpdateHealth);
            }
        }
        
        // Initial update
        UpdateAll();
    }
    
    private void Update()
    {
        // Update stats every frame
        UpdateStats();
    }
    
    /// <summary>
    /// Update all HUD elements
    /// </summary>
    private void UpdateAll()
    {
        if (LevelManager.Instance != null)
        {
            UpdateLevel(LevelManager.Instance.CurrentLevel);
            UpdateExperience(LevelManager.Instance.CurrentExperience, LevelManager.Instance.ExperienceToNextLevel);
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                UpdateHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
        }
    }
    
    /// <summary>
    /// Update health display
    /// </summary>
    private void UpdateHealth(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{current:F0}/{max:F0}";
        }
    }
    
    /// <summary>
    /// Update experience display
    /// </summary>
    private void UpdateExperience(int current, int required)
    {
        if (experienceSlider != null)
        {
            experienceSlider.maxValue = required;
            experienceSlider.value = current;
        }
        
        if (experienceText != null)
        {
            experienceText.text = $"{current}/{required}";
        }
    }
    
    /// <summary>
    /// Update level display
    /// </summary>
    private void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }
    
    /// <summary>
    /// Update stats display
    /// </summary>
    private void UpdateStats()
    {
        if (enemyCountText != null && EnemySpawner.Instance != null)
        {
            enemyCountText.text = $"Enemies: {EnemySpawner.Instance.GetAliveEnemyCount()}";
        }
        
        if (attackCountText != null && PlayerAttackManager.Instance != null)
        {
            attackCountText.text = $"Attacks: {PlayerAttackManager.Instance.GetActiveAttackCount()}";
        }
    }
}
