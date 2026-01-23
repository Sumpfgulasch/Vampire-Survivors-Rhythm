# Centralized Enemy Movement System - Architecture Plan

## Overview
This document outlines the architecture for centralizing enemy movement logic in [`EnemyManager.cs`](../Assets/Scripts/Enemies/EnemyManager.cs) and implementing a move indicator system that shows where enemies will move 1-3 beats before they actually move.

## Current State Analysis

### Enemy.cs Current Implementation
- Each enemy has its own [`OnBeat()`](../Assets/Scripts/Enemies/Enemy.cs:85) method that handles movement individually
- Movement logic includes:
  - Beat counter tracking
  - Individual pathfinding to player
  - Individual collision detection with other enemies
  - Alternative direction calculation when blocked
- Position tracking is partially implemented but incomplete
- Move indicator methods exist but are not fully implemented

### EnemyManager.cs Current Implementation
- Has a skeleton [`OnBeat()`](../Assets/Scripts/Enemies/EnemyManager.cs:34) method
- Already sorts enemies by distance to player
- Has reference to [`moveIndicatorPrefab`](../Assets/Scripts/Enemies/EnemyManager.cs:9)
- Incomplete implementation of move indicator logic

## Proposed Architecture

### 1. Enemy.cs Modifications

#### Remove Individual Movement Logic
- Remove or simplify [`ExecuteBehavior()`](../Assets/Scripts/Enemies/Enemy.cs:124)
- Remove [`MoveTowardTarget()`](../Assets/Scripts/Enemies/Enemy.cs:134) and related pathfinding methods
- Keep only the beat counter increment in [`OnBeat()`](../Assets/Scripts/Enemies/Enemy.cs:85)

#### Add Position Tracking
```csharp
// Current position on grid (Vector2 for 2D grid coordinates)
public Vector2 CurrentGridPosition { get; set; }

// Target position where enemy will move next
public Vector2 TargetGridPosition { get; set; }

// Reference to the move indicator instance
private GridCellIndicator moveIndicator;
```

#### Keep Essential Methods
- [`MustShowMoveIndicator()`](../Assets/Scripts/Enemies/Enemy.cs:105) - Check if indicator should be shown
- [`MustMove()`](../Assets/Scripts/Enemies/Enemy.cs:109) - Check if enemy should move this beat
- [`MoveTo(Vector3)`](../Assets/Scripts/Enemies/Enemy.cs:230) - Execute the actual movement animation
- [`ShowMoveIndicator(Vector3)`](../Assets/Scripts/Enemies/Enemy.cs:313) - Create indicator at position
- [`DestroyMoveIndicator()`](../Assets/Scripts/Enemies/Enemy.cs:317) - Clean up indicator

#### Update Die() Method
- Ensure [`DestroyMoveIndicator()`](../Assets/Scripts/Enemies/Enemy.cs:317) is called in [`Die()`](../Assets/Scripts/Enemies/Enemy.cs:280)

### 2. EnemyManager.cs Implementation

#### OnBeat() Flow
```
1. Loop through all alive enemies
2. Increment beat counters (or let Enemy.OnBeat() handle this)
3. Check for enemies that need move indicators
   - Calculate target positions for these enemies
   - Show indicators
4. Check for enemies that need to move
   - Move them to their previously calculated target positions
```

#### Centralized Position Calculation
```csharp
private Vector3 CalculateFreePosition(Enemy enemy, List<Enemy> sortedEnemies)
{
    // 1. Calculate direction to player
    // 2. Get grid direction (cardinal only)
    // 3. Calculate target position
    // 4. Check if position is occupied by another enemy
    // 5. If occupied, try alternative directions
    // 6. Return free position or current position if none found
}
```

#### Collision Detection
```csharp
private bool IsPositionOccupied(Vector3 position, Enemy excludeEnemy)
{
    // Check if any enemy (except the one being checked) 
    // is at or targeting this position
    foreach (var enemy in enemies)
    {
        if (enemy == excludeEnemy) continue;
        
        // Check current position
        if (Vector3.Distance(enemy.transform.position, position) < threshold)
            return true;
            
        // Check target position (if indicator exists)
        if (enemy.TargetPosition != null && 
            Vector3.Distance(enemy.TargetPosition, position) < threshold)
            return true;
    }
    return false;
}
```

### 3. Move Indicator System

#### Timing Logic
- [`gameConfig.EnemyMoveIndicatorBeats`](../Assets/Scripts/ScriptableObjects/GameConfigSO.cs:31) defines how many beats before movement to show indicator
- Example: If `EnemyMoveIndicatorBeats = 1` and `BeatsToMove = 3`:
  - Beat 0: Enemy spawns
  - Beat 1: Nothing
  - Beat 2: Show move indicator (3 - 1 = 2)
  - Beat 3: Move to indicated position, destroy indicator

#### Indicator Lifecycle
1. **Creation**: When `beatCounter == (BeatsToMove - EnemyMoveIndicatorBeats)`
2. **Display**: Indicator shows at calculated target position
3. **Destruction**: When enemy moves OR when enemy dies

#### GridCellIndicator Component
- Already has [`GridPosition`](../Assets/Scripts/Feedback/GridCellIndicator.cs:6) property
- Has [`SetEmissiveColor()`](../Assets/Scripts/Feedback/GridCellIndicator.cs:8) for visual feedback
- Should be instantiated at target position with slight Y offset

### 4. Grid System Integration

#### Grid Utilities (in EnemyManager)
```csharp
private Vector3 GetGridDirection(Vector3 direction)
{
    // Snap to 4-directional (cardinal only)
    // Already implemented in Enemy.cs, move to EnemyManager
}

private Vector3 SnapToGrid(Vector3 position, float gridSize)
{
    // Snap position to grid
    // Already implemented in Enemy.cs, move to EnemyManager
}

private Vector3[] GetAlternativeGridDirections(Vector3 primaryDirection)
{
    // Get alternative directions when primary is blocked
    // Already implemented in Enemy.cs, move to EnemyManager
}
```

## Implementation Steps

### Step 1: Update Enemy.cs
1. Add proper position tracking properties
2. Simplify [`OnBeat()`](../Assets/Scripts/Enemies/Enemy.cs:85) to only increment counter
3. Remove individual movement calculation methods
4. Keep movement execution method ([`MoveTo()`](../Assets/Scripts/Enemies/Enemy.cs:230))
5. Update [`Die()`](../Assets/Scripts/Enemies/Enemy.cs:280) to destroy move indicator
6. Ensure [`ShowMoveIndicator()`](../Assets/Scripts/Enemies/Enemy.cs:313) and [`DestroyMoveIndicator()`](../Assets/Scripts/Enemies/Enemy.cs:317) work correctly

### Step 2: Implement EnemyManager.OnBeat()
1. Get sorted list of enemies by distance to player
2. First pass: Handle move indicators
   - Loop through enemies
   - Check if [`MustShowMoveIndicator()`](../Assets/Scripts/Enemies/Enemy.cs:105)
   - Calculate free position using centralized logic
   - Store target position in enemy
   - Call [`enemy.ShowMoveIndicator(targetPosition)`](../Assets/Scripts/Enemies/Enemy.cs:313)
3. Second pass: Handle movement
   - Loop through enemies
   - Check if [`MustMove()`](../Assets/Scripts/Enemies/Enemy.cs:109)
   - Move to stored target position
   - Destroy move indicator
   - Reset beat counter

### Step 3: Add Grid Utilities to EnemyManager
1. Move grid-related methods from Enemy.cs to EnemyManager
2. Make them private helper methods
3. Add collision detection that checks both current and target positions

### Step 4: Test and Refine
1. Test with single enemy
2. Test with multiple enemies
3. Test collision avoidance
4. Test indicator timing with different `EnemyMoveIndicatorBeats` values
5. Test indicator cleanup on enemy death

## Key Design Decisions

### Why Centralize?
1. **Performance**: Single pass through enemies instead of N² collision checks
2. **Consistency**: All enemies use same pathfinding logic
3. **Predictability**: Easier to ensure no two enemies target same position
4. **Maintainability**: Movement logic in one place

### Why Two-Pass System?
1. **Indicator Phase**: Calculate and show where enemies will move
2. **Movement Phase**: Execute the moves
3. This ensures indicators are shown before movement happens
4. Allows for future features like player seeing enemy intentions

### Position Tracking Strategy
- `CurrentGridPosition`: Where enemy currently is
- `TargetGridPosition`: Where enemy will move next (shown by indicator)
- This allows collision detection to check both current and future positions

## Edge Cases to Handle

1. **Enemy dies before moving**: Indicator must be destroyed in [`Die()`](../Assets/Scripts/Enemies/Enemy.cs:280)
2. **No free position available**: Enemy stays in place, no indicator shown
3. **Multiple enemies want same position**: First in sorted list gets priority
4. **Enemy spawns mid-beat cycle**: Beat counter starts at 0

## Configuration

### GameConfigSO
- [`EnemyMoveIndicatorBeats`](../Assets/Scripts/ScriptableObjects/GameConfigSO.cs:31): How many beats before movement to show indicator (1-3)

### EnemyTypeSO
- [`BeatsToMove`](../Assets/Scripts/ScriptableObjects/EnemyTypeSO.cs:18): How many beats between movements
- [`MoveDistance`](../Assets/Scripts/ScriptableObjects/EnemyTypeSO.cs:15): Distance to move (grid size)

## Visual Feedback

### Move Indicator
- Glowing cell at target position
- Color: Yellow/Orange to indicate enemy movement
- Intensity: Can pulse or glow steadily
- Position: Slightly above ground (Y offset)

## Future Enhancements

1. **Different indicator colors** for different enemy types
2. **Indicator intensity** increases as move gets closer
3. **Path indicators** showing full path for enemies that move multiple cells
4. **Prediction system** for player to see enemy movements ahead of time
