using UnityEngine;
using DG.Tweening;

/// <summary>
/// Player projectile that moves on beats
/// </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerProjectile : MonoBehaviour
{
    private Vector3 direction;
    private ProjectileAttackSO data;
    private float beatCounter = 0f;
    private int pierceCount = 0;
    private bool isMoving = false;
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    
    /// <summary>
    /// Initialize the projectile
    /// </summary>
    public void Initialize(Vector3 dir, ProjectileAttackSO projectileData)
    {
        direction = dir.normalized;
        data = projectileData;
        
        // Face the direction
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        MoveProjectile();
        
        // Auto-destroy after lifetime
        if (data != null && data.Lifetime > 0)
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
        if (isMoving || data == null) return;
        
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
            .OnComplete(() => {
                isMoving = false;
            });
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(data.Damage);
                pierceCount++;
                FeedbackManager.Instance.TriggerHitFeedback(transform.position);
                
                // Check if we should destroy
                if (data.DestroyOnHit || pierceCount > data.MaxPierceCount)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
