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
    
    private GameConfigSO gameConfig;

    // Properties
    public float CollisionDamage => enemyData != null ? enemyData.CollisionDamage : 0.5f;
    public bool IsDead => isDead;

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
    }

    protected virtual void OnEnable() {
        if (BeatManager.Instance != null) {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
        }
        else {
            FindFirstObjectByType<BeatManager>().OnBeat.AddListener(OnBeat);
        }

        // Find player if not assigned
        if (Target == null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                Target = player.transform;
            }
        }
    }

    protected virtual void OnDisable() {
        if (BeatManager.Instance != null) {
            BeatManager.Instance.OnBeat.RemoveListener(OnBeat);
        }
    }

    /// <summary>
    /// Called on every beat
    /// </summary>
    protected virtual void OnBeat() {
        if (isDead || isMoving) return;

        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive) {
            return;
        }

        beatCounter++;
        
        if (beatCounter - enemyData.BeatsToMove == gameConfig.EnemyMoveIndicatorBeats) {
            ShowMoveIndicator();
        }

        // Check if it's time to move
        if (beatCounter >= enemyData.BeatsToMove) {
            beatCounter = 0;
            ExecuteBehavior();
        }
    }
    
    protected void ShowMoveIndicator() {
        
    }

    /// <summary>
    /// Execute the enemy's behavior (override in derived classes)
    /// </summary>
    protected virtual void ExecuteBehavior() {
        // Default behavior: move toward player
        if (Target != null) {
            MoveTowardTarget();
        }
    }

    /// <summary>
    /// Move toward the target
    /// </summary>
    protected virtual void MoveTowardTarget() {
        if (Target == null || enemyData == null) return;

        // Calculate direction to target
        Vector3 direction = (Target.position - rb.position);
        direction.y = 0f; // Keep on XZ plane

        // Determine grid direction (snap to cardinal/diagonal directions)
        Vector3 gridDirection = GetGridDirection(direction);

        // Calculate target position on grid
        Vector3 targetPosition = rb.position + gridDirection * enemyData.MoveDistance;

        // Snap to grid
        targetPosition = SnapToGrid(targetPosition);

        // Check for collisions with other enemies
        if (CheckCollisionAtPosition(targetPosition)) {
            // Try alternative grid directions in order of preference
            Vector3[] alternativeDirections = GetAlternativeGridDirections(gridDirection);

            foreach (Vector3 altDir in alternativeDirections) {
                Vector3 altPosition = rb.position + altDir * enemyData.MoveDistance;
                altPosition = SnapToGrid(altPosition);

                if (!CheckCollisionAtPosition(altPosition)) {
                    targetPosition = altPosition;
                    break;
                }
            }
        }

        // Move to target position
        MoveTo(targetPosition);
    }

    protected Vector3 GetGridDirection(Vector3 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absZ = Mathf.Abs(direction.z);
    
        // 4-directional (cardinal only)
        if (absX > absZ)
            return new Vector3(Mathf.Sign(direction.x), 0f, 0f);
        else
            return new Vector3(0f, 0f, Mathf.Sign(direction.z));
    }

    protected Vector3 SnapToGrid(Vector3 position) {
        float gridSize = enemyData.MoveDistance; // Or use a separate grid size variable

        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            position.y,
            Mathf.Round(position.z / gridSize) * gridSize
        );
    }

    protected Vector3[] GetAlternativeGridDirections(Vector3 primaryDirection) {
        // Get angle of primary direction
        float primaryAngle = Mathf.Atan2(primaryDirection.z, primaryDirection.x) * Mathf.Rad2Deg;

        // Create array of alternative directions, ordered by preference
        List<Vector3> alternatives = new List<Vector3>();

        // Try directions 45 degrees to either side, then 90, then 135, then opposite
        float[] angleOffsets = { 45f, -45f, 90f, -90f, 135f, -135f, 180f };

        foreach (float offset in angleOffsets) {
            float angle = (primaryAngle + offset) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
            alternatives.Add(dir);
        }

        return alternatives.ToArray();
    }

    /// <summary>
    /// Check if there's a collision at the target position
    /// </summary>
    protected bool CheckCollisionAtPosition(Vector3 position) {
        float checkRadius = 0.4f;
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);

        foreach (Collider other in colliders) {
            if (other != col && other.CompareTag("Enemy")) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Move to a specific position with animation
    /// </summary>
    protected virtual void MoveTo(Vector3 targetPosition) {
        isMoving = true;

        float moveDuration = 0.2f;

        rb.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => { isMoving = false; });
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

        // Drop experience gem
        if (enemyData != null && enemyData.ExperienceValue > 0) {
            CollectibleSpawner.SpawnGem(transform.position, enemyData.ExperienceValue);
        }

        // Death animation
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => { Destroy(gameObject); });
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