using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance;

    [Header("References")]
    public Transform player;
    public GridCellIndicator moveIndicatorPrefab;
    
    [Header("Configuration")]
    [SerializeField] private GameConfigSO gameConfig;
    [SerializeField] private float gridSize = 1f; // Should match enemy move distance
    
    private List<Enemy> enemies => EnemySpawner.Instance?.AliveEnemies;

    void Start() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        
        // Find GameConfig if not assigned
        if (gameConfig == null) {
            gameConfig = Resources.Load<GameConfigSO>("GameConfig");
            if (gameConfig == null) {
                Debug.LogError("EnemyManager: GameConfig not found!");
            }
        }
        
        if (BeatManager.Instance != null) {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
        }
        else {
            FindFirstObjectByType<BeatManager>().OnBeat.AddListener(OnBeat);
        }
    }

    void Update() {
    }

    /// <summary>
    /// Centralized beat handler for all enemy movement
    /// </summary>
    public void OnBeat() {
        if (enemies == null || enemies.Count == 0) return;
        
        // Sort enemies by distance to player (closest first)
        var enemiesByDistance = enemies
            .Where(e => e != null && !e.IsDead)
            .OrderBy(e => Vector3.Distance(e.transform.position, player.position))
            .ToList();
        
        // PHASE 1: Show move indicators for enemies that need them
        foreach (var enemy in enemiesByDistance) {
            enemy.OnBeat();
            if (enemy.MustShowMoveIndicator()) {
                Vector3 targetPosition = CalculateFreePosition(enemy, enemiesByDistance);
                
                // Only show indicator if we found a valid position different from current
                if (Vector3.Distance(targetPosition, enemy.transform.position) > 0.1f) {
                    enemy.ShowMoveIndicator(targetPosition);
                }
            }
        }
        
        // PHASE 2: Move enemies that are ready to move
        foreach (var enemy in enemiesByDistance) {
            if (enemy.MustMove()) {
                // Move to the position indicated by the move indicator
                Vector3 targetPosition = enemy.TargetPosition != Vector2.zero
                    ? new Vector3(enemy.TargetPosition.x, enemy.transform.position.y, enemy.TargetPosition.y)
                    : enemy.transform.position;
                
                enemy.MoveToPosition(targetPosition);
            }
        }
    }
    
    /// <summary>
    /// Calculate a free position for an enemy to move to
    /// </summary>
    private Vector3 CalculateFreePosition(Enemy enemy, List<Enemy> sortedEnemies) {
        if (player == null || enemy == null) return enemy.transform.position;
        
        // Get enemy data for move distance
        float moveDistance = gridSize; // Default
        
        // Calculate direction to player
        Vector3 direction = (player.position - enemy.transform.position);
        direction.y = 0f; // Keep on XZ plane
        
        // Get grid direction (cardinal only)
        Vector3 gridDirection = GetGridDirection(direction);
        
        // Calculate target position
        Vector3 targetPosition = enemy.transform.position + gridDirection * moveDistance;
        targetPosition = SnapToGrid(targetPosition);
        
        // Check if position is free
        if (!IsPositionOccupied(targetPosition, enemy, sortedEnemies)) {
            return targetPosition;
        }
        
        // Try alternative directions
        Vector3[] alternativeDirections = GetAlternativeGridDirections(gridDirection);
        
        foreach (Vector3 altDir in alternativeDirections) {
            Vector3 altPosition = enemy.transform.position + altDir * moveDistance;
            altPosition = SnapToGrid(altPosition);
            
            if (!IsPositionOccupied(altPosition, enemy, sortedEnemies)) {
                return altPosition;
            }
        }
        
        // No free position found, stay in place
        return enemy.transform.position;
    }

    /// <summary>
    /// Check if a position is occupied by another enemy (current or target position)
    /// </summary>
    private bool IsPositionOccupied(Vector3 position, Enemy excludeEnemy, List<Enemy> allEnemies) {
        float checkRadius = 0.4f;
        
        foreach (var enemy in allEnemies) {
            if (enemy == excludeEnemy || enemy == null || enemy.IsDead) continue;
            
            // Check current position
            if (Vector3.Distance(enemy.transform.position, position) < checkRadius) {
                return true;
            }
            
            // Check target position (if enemy has a move indicator)
            if (enemy.TargetPosition != Vector2.zero) {
                Vector3 targetPos = new Vector3(
                    enemy.TargetPosition.x,
                    position.y,
                    enemy.TargetPosition.y
                );
                
                if (Vector3.Distance(targetPos, position) < checkRadius) {
                    return true;
                }
            }
        }
        
        return false;
    }

    /// <summary>
    /// Get grid direction (4-directional, cardinal only)
    /// </summary>
    private Vector3 GetGridDirection(Vector3 direction) {
        float absX = Mathf.Abs(direction.x);
        float absZ = Mathf.Abs(direction.z);

        // 4-directional (cardinal only)
        if (absX > absZ)
            return new Vector3(Mathf.Sign(direction.x), 0f, 0f);
        else
            return new Vector3(0f, 0f, Mathf.Sign(direction.z));
    }

    /// <summary>
    /// Snap position to grid
    /// </summary>
    private Vector3 SnapToGrid(Vector3 position) {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            position.y,
            Mathf.Round(position.z / gridSize) * gridSize
        );
    }

    /// <summary>
    /// Get alternative grid directions when primary is blocked
    /// </summary>
    private Vector3[] GetAlternativeGridDirections(Vector3 primaryDirection) {
        // Get angle of primary direction
        float primaryAngle = Mathf.Atan2(primaryDirection.z, primaryDirection.x) * Mathf.Rad2Deg;

        // Create array of alternative directions, ordered by preference
        List<Vector3> alternatives = new List<Vector3>();

        // Try directions 90 degrees to either side, then opposite
        // For 4-directional movement, we only have 3 alternatives
        float[] angleOffsets = { 90f, -90f, 180f };

        foreach (float offset in angleOffsets) {
            float angle = (primaryAngle + offset) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            
            // Snap to cardinal directions
            dir = GetGridDirection(dir);
            alternatives.Add(dir);
        }

        return alternatives.ToArray();
    }
}