using UnityEngine;
using DG.Tweening;

/// <summary>
/// Base class for all enemies with beat-synchronized behavior
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] protected EnemyTypeSO enemyData;
    
    public Transform Target { get; set; }
    
    [Header("Debug")]
    [SerializeField] protected bool showDebug = false;
    
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

    public void Init(Transform target) {
        Target = target;
    }
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        // Configure rigidbody
        rb.isKinematic = true;
        rb.useGravity = false;
        
        // Initialize health
        if (enemyData != null)
        {
            currentHealth = enemyData.Health;
        }
    }
    
    protected virtual void OnEnable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
        }
        else {
            FindFirstObjectByType<BeatManager>().OnBeat.AddListener(OnBeat);
        }
        
        // Find player if not assigned
        if (Target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Target = player.transform;
            }
        }
    }
    
    protected virtual void OnDisable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.RemoveListener(OnBeat);
        }
    }
    
    /// <summary>
    /// Called on every beat
    /// </summary>
    protected virtual void OnBeat()
    {
        if (isDead || isMoving) return;
        
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive)
        {
            return;
        }
        
        beatCounter++;
        
        // Check if it's time to move
        if (enemyData != null && beatCounter >= enemyData.BeatsToMove)
        {
            beatCounter = 0;
            ExecuteBehavior();
        }
    }
    
    /// <summary>
    /// Execute the enemy's behavior (override in derived classes)
    /// </summary>
    protected virtual void ExecuteBehavior()
    {
        // Default behavior: move toward player
        if (Target != null)
        {
            MoveTowardTarget();
        }
    }
    
    /// <summary>
    /// Move toward the target
    /// </summary>
    protected virtual void MoveTowardTarget()
    {
        if (Target == null || enemyData == null) return;
        
        // Calculate direction to target
        Vector3 direction = (Target.position - rb.position);
        direction.y = 0f; // Keep on XZ plane
        direction.Normalize();
        
        // Calculate target position
        Vector3 targetPosition = rb.position + direction * enemyData.MoveDistance;
        
        // Check for collisions with other enemies
        if (CheckCollisionAtPosition(targetPosition))
        {
            // Try alternative angles
            for (int i = 1; i <= 4; i++)
            {
                float angle = 30f * i;
                Vector3 altDirection = Quaternion.Euler(0f, angle, 0f) * direction;
                Vector3 altPosition = rb.position + altDirection * enemyData.MoveDistance;
                
                if (!CheckCollisionAtPosition(altPosition))
                {
                    targetPosition = altPosition;
                    break;
                }
                
                // Try negative angle
                altDirection = Quaternion.Euler(0f, -angle, 0f) * direction;
                altPosition = rb.position + altDirection * enemyData.MoveDistance;
                
                if (!CheckCollisionAtPosition(altPosition))
                {
                    targetPosition = altPosition;
                    break;
                }
            }
        }
        
        // Move to target position
        MoveTo(targetPosition);
    }
    
    /// <summary>
    /// Check if there's a collision at the target position
    /// </summary>
    protected bool CheckCollisionAtPosition(Vector3 position)
    {
        float checkRadius = 0.4f;
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);
        
        foreach (Collider other in colliders)
        {
            if (other != col && other.CompareTag("Enemy"))
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Move to a specific position with animation
    /// </summary>
    protected virtual void MoveTo(Vector3 targetPosition)
    {
        isMoving = true;
        
        float moveDuration = 0.2f;
        
        rb.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                isMoving = false;
            });
    }
    
    /// <summary>
    /// Take damage
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (showDebug)
        {
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");
        }
        
        // Flash effect
        FlashDamage();
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Flash damage effect
    /// </summary>
    protected virtual void FlashDamage()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.DOColor(Color.white, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => {
                    if (renderer != null)
                    {
                        renderer.material.color = originalColor;
                    }
                });
        }
    }
    
    /// <summary>
    /// Handle death
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;
        
        if (showDebug)
        {
            Debug.Log($"{gameObject.name} died");
        }
        
        // Drop experience gem
        if (enemyData != null && enemyData.ExperienceValue > 0)
        {
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
    public virtual void Initialize(EnemyTypeSO data, Transform playerTarget)
    {
        enemyData = data;
        Target = playerTarget;
        
        if (enemyData != null)
        {
            currentHealth = enemyData.Health;
        }
    }
    
    protected virtual void OnDrawGizmos()
    {
        if (!showDebug) return;
        
        // Draw movement range
        if (enemyData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.MoveDistance);
        }
        
        // Draw line to target
        if (Target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.position);
        }
    }
}
