using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Base class for all enemies with beat-synchronized behavior
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Enemy : MonoBehaviour {
    [Header("Configuration")] [SerializeField]
    protected EnemyTypeSO enemyData;

    public Transform Target { get; set; }

    [Header("Debug")] [SerializeField] protected bool showDebug = false;

    // Components
    protected Rigidbody rb;
    protected Collider col;

    // State
    protected int beatCounter = 0;
    protected float currentHealth;
    protected bool isMoving = false;
    protected bool isDead = false;

    // Properties
    public float CollisionDamage => enemyData != null ? enemyData.CollisionDamage : 0.5f;
    public bool IsDead => isDead;
    
    // Grid position tracking
    public Vector2 CurrentGridPosition { get; set; }
    public Vector2 TargetGridPosition { get; set; }
    public Vector2 Position => CurrentGridPosition;
    public Vector2 TargetPosition => moveIndicator != null ? moveIndicator.GridPosition : CurrentGridPosition;
    
    // private
    private GameConfigSO gameConfig;
    private GridCellIndicator moveIndicator;

    public void Init(Transform target, GameConfigSO gameConfig) {
        Target = target;
        this.gameConfig = gameConfig;
    }

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

    protected virtual void OnEnable() {
        // Find player if not assigned
        if (Target == null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                Target = player.transform;
            }
        }
    }

    protected virtual void OnDisable() {

    }

    /// <summary>
    /// Called on every beat - simplified to only increment counter
    /// EnemyManager handles all movement logic
    /// </summary>
    public virtual void OnBeat() {
        if (isDead || isMoving) return;
        
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive) {
            return;
        }

        beatCounter++;
    }
    
    /// <summary>
    /// Check if this enemy should show move indicator this beat
    /// </summary>
    public bool MustShowMoveIndicator() {
        return beatCounter == (enemyData.BeatsToMove - gameConfig.EnemyMoveIndicatorBeats);
    }
    
    /// <summary>
    /// Check if this enemy should move this beat
    /// </summary>
    public bool MustMove() {
        return beatCounter >= enemyData.BeatsToMove;
    }

    // OLD MOVEMENT METHODS - Now handled by EnemyManager
    // Commented out but kept for reference
    
    /*
    protected virtual void ExecuteBehavior() {
        if (Target != null) {
            MoveTowardTarget();
        }
    }

    protected virtual void MoveTowardTarget() {
        // Movement logic moved to EnemyManager
    }

    protected Vector3 GetGridDirection(Vector3 direction) {
        // Moved to EnemyManager
    }

    protected Vector3 SnapToGrid(Vector3 position) {
        // Moved to EnemyManager
    }

    protected Vector3[] GetAlternativeGridDirections(Vector3 primaryDirection) {
        // Moved to EnemyManager
    }

    protected bool CheckCollisionAtPosition(Vector3 position) {
        // Moved to EnemyManager
    }
    */

    /// <summary>
    /// Move to a specific position with animation
    /// </summary>
    protected virtual void MoveTo(Vector3 targetPosition, Action onComplete = null) {
        isMoving = true;
        
        // Update current grid position
        CurrentGridPosition = new Vector2(targetPosition.x, targetPosition.z);

        float moveDuration = 0.2f;

        rb.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                isMoving = false;
                onComplete?.Invoke();
            });
    }
    
    /// <summary>
    /// Public wrapper for EnemyManager to call - moves and resets beat counter
    /// </summary>
    public void MoveToPosition(Vector3 targetPosition) {
        MoveTo(targetPosition, DestroyMoveIndicator);
        beatCounter = 0;
    }

    /// <summary>
    /// Take damage
    /// </summary>
    public virtual void TakeDamage(float damage) {
        if (isDead) return;

        currentHealth -= damage;

        if (showDebug) {
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");
        }

        // Flash effect
        FlashDamage();

        if (currentHealth <= 0f) {
            Die();
        }
    }

    /// <summary>
    /// Flash damage effect
    /// </summary>
    protected virtual void FlashDamage() {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) {
            Color originalColor = renderer.material.color;
            renderer.material.DOColor(Color.white, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => {
                    if (renderer != null) {
                        renderer.material.color = originalColor;
                    }
                });
        }
    }

    /// <summary>
    /// Handle death
    /// </summary>
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

    /// <summary>
    /// Initialize the enemy with data
    /// </summary>
    public virtual void Initialize(EnemyTypeSO data, Transform playerTarget) {
        enemyData = data;
        Target = playerTarget;

        if (enemyData != null) {
            currentHealth = enemyData.Health;
        }
    }

    /// <summary>
    /// Show move indicator at target position - called by EnemyManager
    /// </summary>
    public void ShowMoveIndicator(Vector3 targetPosition) {
        // Destroy existing indicator if any
        if (moveIndicator != null) {
            DestroyMoveIndicator();
        }
        
        var moveIndicatorPos = new Vector3(targetPosition.x, gameConfig.MoveIndicatorYOffset, targetPosition.z);
        
        moveIndicator = Instantiate(
            EnemyManager.Instance.moveIndicatorPrefab,
            moveIndicatorPos,
            Quaternion.identity
        );
        
        // Store grid position
        TargetGridPosition = new Vector2(targetPosition.x, targetPosition.z);
        moveIndicator.GridPosition = TargetGridPosition;
        
        // Optional: Set indicator color/intensity
        //moveIndicator.SetEmissiveColor(Color.yellow, 2f);
    }

    /// <summary>
    /// Destroy move indicator - called when moving or dying
    /// </summary>
    public void DestroyMoveIndicator() {
        if (moveIndicator == null)
            return;
        
        Destroy(moveIndicator.gameObject);
        moveIndicator = null;
    }

    protected virtual void OnDrawGizmos() {
        if (!showDebug) return;

        // Draw movement range
        if (enemyData != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.MoveDistance);
        }

        // Draw line to target
        if (Target != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.position);
        }
    }
}