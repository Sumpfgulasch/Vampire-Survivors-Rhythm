using UnityEngine;

/// <summary>
/// Global game configuration settings
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Rhythm Game/Game Config")]
public class GameConfigSO : ScriptableObject
{
    [Header("Rhythm Settings")]
    [Tooltip("Beats per minute for the music track")]
    [Range(60f, 200f)]
    public float BPM = 120f;
    [Range(1, 12)]
    public int BeatsPerBar = 4;
    
    [Tooltip("Tolerance window for 'on-beat' actions (in seconds)")]
    [Range(0.05f, 0.3f)]
    public float BeatTolerance = 0.1f;
    
    [Header("Player Settings")]
    [Tooltip("Starting health for the player")]
    public float StartingPlayerHealth = 3f;
    
    [Tooltip("Player movement distance per beat")]
    public float PlayerMoveDistance = 1f;
    
    [Tooltip("How many beats between player movements")]
    public int PlayerBeatsPerMove = 2;

    [Header("Enemy Settings")] 
    public int EnemyMoveIndicatorBeats = 1;
    [Tooltip("Distance from player where enemies spawn")] public float SpawnRadius = 15f;
    [Tooltip("Minimum distance from player for spawning")] public float MinSpawnDistance = 10f;
    public float MoveIndicatorYOffset = 0.02f;
    
    [Header("Gameplay Settings")]
    [Tooltip("Duration of invincibility after taking damage (in seconds)")]
    public float InvincibilityDuration = 1f;
    
    [Tooltip("Gem collection radius")]
    public float GemCollectionRadius = 1.5f;
    
    /// <summary>
    /// Calculate seconds per beat based on BPM
    /// </summary>
    public float SecondsPerBeat => 60f / BPM;
}
