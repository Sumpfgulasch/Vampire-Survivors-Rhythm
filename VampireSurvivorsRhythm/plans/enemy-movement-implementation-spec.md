# Enemy Movement Implementation Specification

## File: Enemy.cs

### Properties to Add/Modify

```csharp
// Add these properties
public Vector2 CurrentGridPosition { get; set; }
public Vector2 TargetGridPosition { get; set; }

// Modify existing property
public Vector2 Position => CurrentGridPosition;

// Keep existing
public Vector2 TargetPosition => moveIndicator != null ? moveIndicator.GridPosition : Position;
```

### OnBeat() Method - Simplified Version

```csharp
protected virtual void OnBeat() {
    if (isDead || isMoving) return;
    
    if (!GameStateManager.Instance.IsGameplayActive) {
        return;
    }

    beatCounter++;
    
    // That's it! EnemyManager handles the rest
}
```

### Methods to Keep (with modifications)

```csharp
// Keep - used by EnemyManager to check timing
public bool MustShowMoveIndicator() {
    return beatCounter == (enemyData.BeatsToMove - gameConfig.EnemyMoveIndicatorBeats);
}

// Keep - used by EnemyManager to check timing
public bool MustMove() {
    return beatCounter >= enemyData.BeatsToMove;
}

// Keep - called by EnemyManager to show indicator
public void ShowMoveIndicator(Vector3 targetPosition) {
    if (moveIndicator != null) {
        DestroyMoveIndicator();
    }
    
    moveIndicator = Instantiate(
        EnemyManager.Instance.moveIndicatorPrefab, 
        targetPosition, 
        Quaternion.identity
    );
    
    // Store grid position
    TargetGridPosition = new Vector2(targetPosition.x, targetPosition.z);
    
    // Optional: Set indicator color/intensity
    moveIndicator.SetEmissiveColor(Color.yellow, 2f);
}

// Keep - called by EnemyManager and Die()
public void DestroyMoveIndicator() {
    if (moveIndicator == null) 
        return;
    
    Destroy(moveIndicator.gameObject);
    moveIndicator = null;
}

// Keep - called by EnemyManager to execute movement
protected virtual void MoveTo(Vector3 targetPosition) {
    isMoving = true;
    
    // Update current grid position
    CurrentGridPosition = new Vector2(targetPosition.x, targetPosition.z);

    float moveDuration = 0.2f;

    rb.DOMove(targetPosition, moveDuration)
        .SetEase(Ease.OutQuad)
        .OnComplete(() => { 
            isMoving = false;
        });
}

// Add - public wrapper for EnemyManager to call
public void MoveToPosition(Vector3 targetPosition) {
    MoveTo(targetPosition);
    DestroyMoveIndicator();
    beatCounter = 0; // Reset counter after move
}
```

### Methods to Remove/Comment Out

```csharp
// Remove or comment out these methods:
// - ExecuteBehavior()
// - MoveTowardTarget()
// - GetGridDirection()
// - SnapToGrid()
// - GetAlternativeGridDirections()
// - CheckCollisionAtPosition()
```

### Die() Method - Add Indicator Cleanup

```csharp
protected virtual void Die() {
    isDead = true;

    if (showDebug) {
        Debug.Log($"{gameObject.name} died");
    }
    
    // Destroy move indicator if it exists
    DestroyMoveIndicator();

    // Drop experience gem
    if (enemyData != null && enemyData.ExperienceValue > 0) {
        CollectibleSpawner.SpawnGem(transform.position, enemyData.ExperienceValue);
    }

    // Death animation
    transform.DOScale(Vector3.zero, 0.3f)
        .SetEase(Ease.InBack)
        .OnComplete(() => {
            Destroy(gameObject);
        });
}
```

### Awake() - Initialize Grid Position

```csharp
protected virtual void Awake() {
    rb = GetComponent<Rigidbody>();
    col = GetComponent<Collider>();

    // Configure rigidbody
    rb.isKinematic = true;
    rb.useGravity = false;

    // Initialize health
    if (enemyData != null) {
        currentHealth = enemyData.Health;
    }
    else {
        Debug.LogError("Enemy: No enemy data assigned!");
    }
    
    // Initialize grid position
    CurrentGridPosition = new Vector2(transform.position.x, transform.position.z);
    TargetGridPosition = CurrentGridPosition;
}
```

---

## File: EnemyManager.cs

### Properties to Add

```csharp
[SerializeField] private GameConfigSO gameConfig;
private float gridSize = 1f; // Should match enemy move distance
```

### OnBeat() Method - Complete Implementation

```csharp
public void OnBeat() {
    if (enemies == null || enemies.Count == 0) return;
    
    // Sort enemies by distance to player (closest first)
    var enemiesByDistance = enemies
        .Where(e => e != null && !e.IsDead)
        .OrderBy(e => Vector3.Distance(e.transform.position, player.position))
        .ToList();
    
    // PHASE 1: Show move indicators for enemies that need them
    foreach (var enemy in enemiesByDistance) {
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
```

### Helper Methods to Add

```csharp
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
```

### Start() Method - Add GameConfig Reference

```csharp
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
```

---

## File: GridCellIndicator.cs

### Modifications (if needed)

```csharp
using UnityEngine;

public class GridCellIndicator : MonoBehaviour {
    [SerializeField] private Material material;
    
    public Vector2 GridPosition { get; set; }
    
    public void SetEmissiveColor(Color color, float intensity) {
        if (material == null) {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null) {
                material = renderer.material;
            }
        }
        
        if (material != null) {
            material.color = color;
            material.SetColor("_EmissionColor", color * Mathf.Pow(2f, intensity));
        }
    }
    
    private void Start() {
        // Store grid position from world position
        GridPosition = new Vector2(transform.position.x, transform.position.z);
    }
}
```

---

## Testing Checklist

### Single Enemy Tests
- [ ] Enemy spawns and beat counter increments
- [ ] Move indicator appears at correct beat (BeatsToMove - EnemyMoveIndicatorBeats)
- [ ] Enemy moves to indicated position at correct beat
- [ ] Indicator is destroyed after movement
- [ ] Enemy moves toward player correctly

### Multiple Enemy Tests
- [ ] Multiple enemies don't target the same position
- [ ] Closest enemy gets priority for positions
- [ ] Enemies find alternative paths when blocked
- [ ] Indicators show correctly for all enemies

### Edge Case Tests
- [ ] Enemy dies before moving - indicator is destroyed
- [ ] No free position available - enemy stays in place
- [ ] Enemy spawns mid-cycle - beat counter works correctly
- [ ] Indicator timing works with different EnemyMoveIndicatorBeats values (1, 2, 3)

### Visual Tests
- [ ] Indicators are visible and positioned correctly
- [ ] Indicators have correct color/glow
- [ ] Movement animation is smooth
- [ ] No visual glitches or overlapping indicators

---

## Configuration Values

### GameConfigSO
- `EnemyMoveIndicatorBeats = 1` (default, can be 1-3)

### EnemyTypeSO
- `BeatsToMove = 3` (example value)
- `MoveDistance = 1f` (should match grid size)

### EnemyManager
- `gridSize = 1f` (should match MoveDistance)

---

## Implementation Order

1. **Update Enemy.cs**
   - Add position tracking properties
   - Simplify OnBeat()
   - Update/add methods for indicators and movement
   - Update Die() to clean up indicators
   - Comment out old movement methods

2. **Update EnemyManager.cs**
   - Add gameConfig reference
   - Implement OnBeat() with two-phase system
   - Add all helper methods for pathfinding and collision

3. **Test with single enemy**
   - Verify indicator timing
   - Verify movement execution
   - Verify indicator cleanup

4. **Test with multiple enemies**
   - Verify collision avoidance
   - Verify position priority
   - Verify all indicators work correctly

5. **Polish and edge cases**
   - Test enemy death scenarios
   - Test different configuration values
   - Verify visual feedback
