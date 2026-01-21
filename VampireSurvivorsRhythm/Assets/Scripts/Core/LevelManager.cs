using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Manages player level progression and experience
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    [Header("Configuration")]
    [SerializeField] private List<StageConfigSO> stageConfigs = new List<StageConfigSO>();
    
    [Header("Events")]
    public UnityEvent<int> OnLevelUp { get; private set; } = new();
    public UnityEvent<int, int> OnExperienceChanged { get; private set; } = new(); // current, required
    
    [Header("Current Progress")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExperience = 0;
    
    public int CurrentLevel => currentLevel;
    public int CurrentExperience => currentExperience;
    public StageConfigSO CurrentStageConfig { get; private set; }
    public int ExperienceToNextLevel => CurrentStageConfig != null ? CurrentStageConfig.ExperienceToNextLevel : 10;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        LoadStageConfig(currentLevel);
    }
    
    /// <summary>
    /// Load the configuration for a specific stage
    /// </summary>
    private void LoadStageConfig(int level)
    {
        // Find the appropriate stage config
        // Use the highest available config if level exceeds available configs
        StageConfigSO config = null;
        
        foreach (var stageConfig in stageConfigs)
        {
            if (stageConfig.StageNumber == level)
            {
                config = stageConfig;
                break;
            }
        }
        
        // If no exact match, use the last available config
        if (config == null && stageConfigs.Count > 0)
        {
            config = stageConfigs[stageConfigs.Count - 1];
            Debug.LogWarning($"No stage config found for level {level}, using stage {config.StageNumber}");
        }
        
        CurrentStageConfig = config;
        
        if (CurrentStageConfig == null)
        {
            Debug.LogError("LevelManager: No stage configs assigned!");
        }
        else
        {
            Debug.Log($"Loaded stage config for level {level}: {CurrentStageConfig.name}");
        }
    }
    
    /// <summary>
    /// Add experience to the player
    /// </summary>
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        OnExperienceChanged?.Invoke(currentExperience, ExperienceToNextLevel);
        
        Debug.Log($"Experience gained: +{amount} (Total: {currentExperience}/{ExperienceToNextLevel})");
        
        // Check for level up
        if (currentExperience >= ExperienceToNextLevel)
        {
            LevelUp();
        }
    }
    
    /// <summary>
    /// Level up the player
    /// </summary>
    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= ExperienceToNextLevel; // Carry over excess experience
        
        Debug.Log($"Level Up! Now level {currentLevel}");
        
        // Load new stage config
        LoadStageConfig(currentLevel);
        
        // Emit level up event
        OnLevelUp?.Invoke(currentLevel);
        
        // Show upgrade screen
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ShowUpgradeScreen();
        }
        
        // Update experience UI with new requirements
        OnExperienceChanged?.Invoke(currentExperience, ExperienceToNextLevel);
    }
    
    /// <summary>
    /// Reset level and experience (for game restart)
    /// </summary>
    public void ResetProgress()
    {
        currentLevel = 1;
        currentExperience = 0;
        LoadStageConfig(currentLevel);
        OnExperienceChanged?.Invoke(currentExperience, ExperienceToNextLevel);
    }
    
    /// <summary>
    /// Get a random upgrade option from the current stage
    /// </summary>
    public AttackUpgradeSO GetRandomUpgrade()
    {
        if (CurrentStageConfig == null || CurrentStageConfig.AvailableUpgrades.Count == 0)
        {
            return null;
        }
        
        int randomIndex = Random.Range(0, CurrentStageConfig.AvailableUpgrades.Count);
        return CurrentStageConfig.AvailableUpgrades[randomIndex];
    }
    
    /// <summary>
    /// Get a random new attack from the current stage
    /// </summary>
    public AttackTypeSO GetRandomNewAttack()
    {
        if (CurrentStageConfig == null || CurrentStageConfig.AvailableNewAttacks.Count == 0)
        {
            return null;
        }
        
        int randomIndex = Random.Range(0, CurrentStageConfig.AvailableNewAttacks.Count);
        return CurrentStageConfig.AvailableNewAttacks[randomIndex];
    }
}
