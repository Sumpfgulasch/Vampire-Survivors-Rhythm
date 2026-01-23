using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Spawns enemies based on stage configuration
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    
    [Header("Configuration")]
    [SerializeField] private GameConfigSO gameConfig;
    [SerializeField] private Transform player;
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = false;
    
    private int beatCounter = 0;
    private List<Enemy> aliveEnemies = new();
    public List<Enemy> AliveEnemies => aliveEnemies.Where(e => e != null).ToList();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }
    
    private void OnEnable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
        }
        else {
            FindFirstObjectByType<BeatManager>().OnBeat.AddListener(OnBeat);
        }
    }
    
    private void OnDisable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.RemoveListener(OnBeat);
        }
    }
    
    private void OnBeat()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive)
        {
            return;
        }
        
        beatCounter++;
        
        StageConfigSO stage = LevelManager.Instance?.CurrentStageConfig;
        if (stage == null) return;
        
        // Clean up dead enemies
        aliveEnemies.RemoveAll(e => e.gameObject == null);
        
        // Check if we should spawn
        if (beatCounter >= stage.BeatsPerSpawn && aliveEnemies.Count < stage.MaxEnemiesAlive)
        {
            beatCounter = 0;
            SpawnEnemy();
        }
    }
    
    /// <summary>
    /// Spawn a random enemy from the current stage
    /// </summary>
    private void SpawnEnemy()
    {
        StageConfigSO stage = LevelManager.Instance?.CurrentStageConfig;
        if (stage == null || stage.EnemySpawns.Count == 0)
        {
            if (showDebug)
            {
                Debug.LogWarning("EnemySpawner: No stage config or enemy spawns available");
            }
            return;
        }
        
        // Select random enemy type based on weights
        EnemySpawnData spawnData = SelectRandomEnemy(stage.EnemySpawns);
        if (spawnData == null || spawnData.EnemyType == null || spawnData.EnemyType.Prefab == null)
        {
            if (showDebug)
            {
                Debug.LogWarning("EnemySpawner: Invalid spawn data");
            }
            return;
        }
        
        // Calculate spawn position (circle around player)
        Vector3 spawnPos = GetRandomSpawnPosition();
        
        // Spawn enemy
        GameObject enemyObj = Instantiate(spawnData.EnemyType.Prefab, spawnPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.Init(player, gameConfig);
        
        if (enemy != null)
        {
            enemy.Initialize(spawnData.EnemyType, player);
        }
        
        aliveEnemies.Add(enemy);
        
        if (showDebug)
        {
            Debug.Log($"Spawned {spawnData.EnemyType.EnemyName} at {spawnPos}. Total enemies: {aliveEnemies.Count}");
        }
    }
    
    /// <summary>
    /// Select a random enemy based on spawn weights
    /// </summary>
    private EnemySpawnData SelectRandomEnemy(List<EnemySpawnData> spawns)
    {
        float totalWeight = 0f;
        foreach (var spawn in spawns)
        {
            totalWeight += spawn.SpawnWeight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (var spawn in spawns)
        {
            currentWeight += spawn.SpawnWeight;
            if (randomValue <= currentWeight)
            {
                return spawn;
            }
        }
        
        return spawns[0]; // Fallback
    }
    
    /// <summary>
    /// Get a random spawn position around the player
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        if (player == null) return Vector3.zero;
        
        float spawnRadius = gameConfig != null ? gameConfig.SpawnRadius : 15f;
        float minDistance = gameConfig != null ? gameConfig.MinSpawnDistance : 10f;
        
        // Random angle
        float angle = Random.Range(0f, 360f);
        
        // Random distance between min and max
        float distance = Random.Range(minDistance, spawnRadius);
        
        // Calculate position
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * distance;
        
        return player.position + offset;
    }
    
    /// <summary>
    /// Get the count of alive enemies
    /// </summary>
    public int GetAliveEnemyCount()
    {
        aliveEnemies.RemoveAll(e => e.gameObject == null);
        return aliveEnemies.Count;
    }
    
    private void OnDrawGizmos()
    {
        if (!showDebug || player == null || gameConfig == null) return;
        
        // Draw spawn radius
        Gizmos.color = Color.yellow;
        DrawCircle(player.position, gameConfig.SpawnRadius);
        
        // Draw min spawn distance
        Gizmos.color = Color.red;
        DrawCircle(player.position, gameConfig.MinSpawnDistance);
    }
    
    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32;
        float angleStep = 360f / segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), 0f, Mathf.Sin(angle1)) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), 0f, Mathf.Sin(angle2)) * radius;
            
            Gizmos.DrawLine(point1, point2);
        }
    }
}
