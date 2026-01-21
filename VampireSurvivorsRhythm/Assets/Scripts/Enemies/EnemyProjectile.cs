using UnityEngine;
using DG.Tweening;

/// <summary>
/// Projectile fired by ranged enemies
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float damage = 0.5f;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float beatsToMove = 0.5f;
    [SerializeField] private float lifetime = 5f;
    
    private Vector3 direction;
    private float beatCounter = 0f;
    private bool isMoving = false;
    private Rigidbody rb;
    
    public float Damage => damage;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
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
    
    /// <summary>
    /// Initialize the projectile
    /// </summary>
    public void Initialize(Vector3 dir, float dmg, float moveDist, float beatsMove)
    {
        direction = dir.normalized;
        damage = dmg;
        moveDistance = moveDist;
        beatsToMove = beatsMove;
        
        // Face the direction
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }
    
    private void OnBeat()
    {
        if (isMoving) return;
        
        beatCounter++;
        
        if (beatCounter >= beatsToMove)
        {
            beatCounter = 0;
            MoveProjectile();
        }
    }
    
    private void MoveProjectile()
    {
        isMoving = true;
        
        Vector3 targetPosition = transform.position + direction * moveDistance;
        
        transform.DOMove(targetPosition, 0.2f)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                isMoving = false;
            });
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Hit player
        if (other.CompareTag("Player"))
        {
            // Damage is handled by PlayerHealth
            Destroy(gameObject);
        }
        
        // Hit wall or obstacle
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
