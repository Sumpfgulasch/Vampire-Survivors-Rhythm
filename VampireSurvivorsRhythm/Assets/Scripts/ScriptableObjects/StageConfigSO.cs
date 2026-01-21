using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Configuration for a game stage/level
/// </summary>
[CreateAssetMenu(fileName = "StageConfig", menuName = "Rhythm Game/Stage Config")]
public class StageConfigSO : ScriptableObject
{
    [Header("Stage Identity")]
    public int StageNumber = 1;
    
    [Header("Enemy Spawning")]
    [Tooltip("Enemy types and their spawn counts for this stage")]
    public List<EnemySpawnData> EnemySpawns = new List<EnemySpawnData>();
    
    [Tooltip("How many beats between enemy spawns")]
    public int BeatsPerSpawn = 4;
    
    [Tooltip("Maximum number of enemies alive at once")]
    public int MaxEnemiesAlive = 20;
    
    [Header("Progression")]
    [Tooltip("Experience needed to reach the next level")]
    public int ExperienceToNextLevel = 10;
    
    [Header("Available Upgrades")]
    [Tooltip("Upgrades that can appear in this stage")]
    public List<AttackUpgradeSO> AvailableUpgrades = new List<AttackUpgradeSO>();
    
    [Tooltip("New attacks that can appear in this stage")]
    public List<AttackTypeSO> AvailableNewAttacks = new List<AttackTypeSO>();
}

/// <summary>
/// Data structure for enemy spawn configuration
/// </summary>
[System.Serializable]
public class EnemySpawnData
{
    public EnemyTypeSO EnemyType;
    
    [Tooltip("Number of this enemy type to spawn in this stage")]
    public int Count = 1;
    
    [Tooltip("Weight for random selection (higher = more likely)")]
    [Range(0f, 1f)]
    public float SpawnWeight = 1f;
}
