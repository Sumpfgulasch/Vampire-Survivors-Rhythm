# Next Steps to Complete the Project

## ✅ What's Been Created (18/30 files - 60%)

### Complete Systems:
- ✅ All ScriptableObjects (6 files)
- ✅ Core Systems (3 files)
- ✅ Player System (2 files)
- ✅ Enemy System (5 files)
- ✅ Spawning System (2 files - partial)

## ⏳ Remaining Critical Files (12 files)

### 1. Enemy Spawner (CRITICAL)
**File**: `Assets/Scripts/Spawning/EnemySpawner.cs`

```csharp
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameConfigSO gameConfig;
    [SerializeField] private Transform player;
    
    private int beatCounter = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    
    private void OnEnable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
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
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive) return;
        
        beatCounter++;
        
        StageConfigSO stage = LevelManager.Instance?.CurrentStageConfig;
        if (stage == null) return;
        
        // Clean up dead enemies
        aliveEnemies.RemoveAll(e => e == null);
        
        // Check if we should spawn
        if (beatCounter >= stage.BeatsPerSpawn && aliveEnemies.Count < stage.MaxEnemiesAlive)
        {
            beatCounter = 0;
            SpawnEnemy();
        }
    }
    
    private void SpawnEnemy()
    {
        StageConfigSO stage = LevelManager.Instance?.CurrentStageConfig;
        if (stage == null || stage.EnemySpawns.Count == 0) return;
        
        // Select random enemy type
        EnemySpawnData spawnData = stage.EnemySpawns[Random.Range(0, stage.EnemySpawns.Count)];
        if (spawnData.EnemyType == null || spawnData.EnemyType.Prefab == null) return;
        
        // Calculate spawn position (circle around player)
        float angle = Random.Range(0f, 360f);
        float distance = gameConfig != null ? gameConfig.SpawnRadius : 15f;
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * distance;
        
        Vector3 spawnPos = player.position + offset;
        
        // Spawn enemy
        GameObject enemyObj = Instantiate(spawnData.EnemyType.Prefab, spawnPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(spawnData.EnemyType, player);
        }
        
        aliveEnemies.Add(enemyObj);
    }
}
```

### 2. Attack Manager (CRITICAL)
**File**: `Assets/Scripts/Attacks/AttackManager.cs`

```csharp
using UnityEngine;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance { get; private set; }
    
    [SerializeField] private Transform player;
    [SerializeField] private List<AttackTypeSO> startingAttacks = new List<AttackTypeSO>();
    
    private List<AttackInstance> activeAttacks = new List<AttackInstance>();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void Start()
    {
        // Add starting attacks
        foreach (var attack in startingAttacks)
        {
            AddAttack(attack);
        }
    }
    
    public void AddAttack(AttackTypeSO attackData)
    {
        if (attackData == null) return;
        
        GameObject attackObj = new GameObject($"Attack_{attackData.AttackName}");
        attackObj.transform.SetParent(transform);
        
        AttackInstance attack = null;
        
        switch (attackData.Category)
        {
            case AttackCategory.Projectile:
                attack = attackObj.AddComponent<ProjectileAttack>();
                break;
            // Add other attack types here when implemented
        }
        
        if (attack != null)
        {
            attack.Initialize(attackData, player);
            activeAttacks.Add(attack);
        }
    }
    
    public void ApplyUpgrade(AttackUpgradeSO upgrade)
    {
        // Apply upgrade logic here
        Debug.Log($"Applied upgrade: {upgrade.UpgradeName}");
    }
}
```

### 3. Attack Instance Base (CRITICAL)
**File**: `Assets/Scripts/Attacks/AttackInstance.cs`

```csharp
using UnityEngine;

public abstract class AttackInstance : MonoBehaviour
{
    protected AttackTypeSO attackData;
    protected Transform player;
    protected int beatCounter = 0;
    
    public virtual void Initialize(AttackTypeSO data, Transform playerTransform)
    {
        attackData = data;
        player = playerTransform;
    }
    
    protected virtual void OnEnable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
        }
    }
    
    protected virtual void OnDisable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.RemoveListener(OnBeat);
        }
    }
    
    protected virtual void OnBeat()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive) return;
        
        beatCounter++;
        
        if (beatCounter >= attackData.BeatsToSpawn)
        {
            beatCounter = 0;
            ExecuteAttack();
        }
    }
    
    protected abstract void ExecuteAttack();
}
```

### 4. Projectile Attack (CRITICAL)
**File**: `Assets/Scripts/Attacks/ProjectileAttack.cs`

```csharp
using UnityEngine;

public class ProjectileAttack : AttackInstance
{
    protected override void ExecuteAttack()
    {
        if (player == null || attackData == null) return;
        
        ProjectileAttackSO projectileData = attackData as ProjectileAttackSO;
        if (projectileData == null || projectileData.VisualPrefab == null) return;
        
        // Get player facing direction
        PlayerController playerController = player.GetComponent<PlayerController>();
        Vector3 direction = playerController != null ? playerController.GetFacingDirection() : Vector3.forward;
        
        // Spawn projectile
        GameObject projObj = Instantiate(projectileData.VisualPrefab, player.position, Quaternion.identity);
        PlayerProjectile proj = projObj.AddComponent<PlayerProjectile>();
        proj.Initialize(direction, projectileData);
    }
}
```

### 5. Player Projectile
**File**: `Assets/Scripts/Attacks/PlayerProjectile.cs`

```csharp
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class PlayerProjectile : MonoBehaviour
{
    private Vector3 direction;
    private ProjectileAttackSO data;
    private float beatCounter = 0f;
    private int pierceCount = 0;
    private bool isMoving = false;
    
    public void Initialize(Vector3 dir, ProjectileAttackSO projectileData)
    {
        direction = dir.normalized;
        data = projectileData;
        
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
        
        if (data.Lifetime > 0)
        {
            Destroy(gameObject, data.Lifetime);
        }
    }
    
    private void OnEnable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
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
        if (isMoving) return;
        
        beatCounter++;
        
        if (beatCounter >= data.BeatsToMove)
        {
            beatCounter = 0;
            MoveProjectile();
        }
    }
    
    private void MoveProjectile()
    {
        isMoving = true;
        Vector3 targetPos = transform.position + direction * data.MoveDistance;
        
        transform.DOMove(targetPos, 0.2f)
            .SetEase(Ease.Linear)
            .OnComplete(() => isMoving = false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(data.Damage);
                pierceCount++;
                
                if (pierceCount > data.MaxPierceCount)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
```

### 6. Camera Controller
**File**: `Assets/Scripts/Camera/CameraController.cs`

```csharp
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float fixedHeight = 10f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -5f);
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 targetPos = new Vector3(
            target.position.x + offset.x,
            fixedHeight,
            target.position.z + offset.z
        );
        
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
```

### 7-10. UI Scripts (Simplified versions)
Create these in `Assets/Scripts/UI/`:
- `HUDManager.cs` - Display health/XP bars
- `UpgradeScreenUI.cs` - Show 3 upgrade options
- `GameOverUI.cs` - Show game over screen
- `BeatFeedbackUI.cs` - Pulse screen on beat

### 11. Feedback Manager (Optional)
**File**: `Assets/Scripts/Feedback/FeedbackManager.cs` - Visual effects

### 12. Editor Script (MOST IMPORTANT)
See separate file: `EDITOR_SCRIPT_TEMPLATE.md`

## 🎯 Priority Order

1. **EnemySpawner** - Without this, no enemies spawn
2. **AttackManager + AttackInstance + ProjectileAttack + PlayerProjectile** - Without this, player can't attack
3. **CameraController** - For proper view
4. **UI Scripts** - For feedback and progression
5. **Editor Script** - To set everything up automatically

## 🚀 Quick Start After Completion

1. Create all remaining scripts above
2. Run the Editor Script (will be provided separately)
3. Assign references in Unity Inspector
4. Create an InputActions asset with "Move" action
5. Set up tags: Player, Enemy, EnemyProjectile, ExperienceGem
6. Press Play!

## 📝 Notes

- All scripts follow the same patterns established
- Beat synchronization via BeatManager.OnBeat
- State checking via GameStateManager
- Configuration via ScriptableObjects
- Animations via DOTween

## 🐛 Common Issues

1. **Missing References**: Use Editor Script to set them up
2. **No Input**: Create InputActions asset
3. **No Tags**: Set up in Unity Tag Manager
4. **No Prefabs**: Editor Script will create them

## 💡 Testing Strategy

1. Test BeatManager first (should see beat counter in debug)
2. Test Player movement (should move every 2 beats)
3. Test Enemy spawning (should spawn around player)
4. Test Attacks (projectiles should spawn and move)
5. Test full loop (kill enemies, collect gems, level up)
