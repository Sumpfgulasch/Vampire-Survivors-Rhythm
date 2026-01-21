using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

/// <summary>
/// Controls player movement synchronized to the beat
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfigSO gameConfig;
    
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private int beatsPerMove = 2;
    
    [Header("Animation Settings")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float jumpDuration = 0.3f;
    [SerializeField] private float rotationAmount = 90f;
    
    // Components
    private Rigidbody rb;
    private PlayerHealth playerHealth;
    
    // Movement state
    private Vector3 lastInputDirection;
    private int beatCounter = 0;
    private bool isMoving = false;
    
    public Vector3 Position => transform.position;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<PlayerHealth>();
        
        // Configure rigidbody
        rb.isKinematic = true;
        rb.useGravity = false;
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
        
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }
    
    private void OnDisable()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.RemoveListener(OnBeat);
        }
        
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }
    
    private void Update()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive)
        {
            return;
        }
        
        // Read input every frame and store direction
        ReadInput();
    }
    
    /// <summary>
    /// Read input from the Input System
    /// </summary>
    private void ReadInput()
    {
        if (moveAction == null || moveAction.action == null) return;
        
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        
        if (input.sqrMagnitude > 0.01f)
        {
            // Convert 2D input to 3D direction (XZ plane)
            lastInputDirection = new Vector3(input.x, 0f, input.y).normalized;
        }
    }
    
    /// <summary>
    /// Called on every beat
    /// </summary>
    private void OnBeat()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive)
        {
            return;
        }
        
        if (isMoving) 
            return;
        
        beatCounter++;
        
        // Check if it's time to move
        if (beatCounter >= beatsPerMove)
        {
            beatCounter = 0;
            
            if (lastInputDirection.sqrMagnitude > 0.01f)
            {
                MovePlayer();
            }
        }
    }
    
    /// <summary>
    /// Move the player in the stored direction
    /// </summary>
    private void MovePlayer()
    {
        if (isMoving) 
            return;
        
        float distance = gameConfig != null ? gameConfig.PlayerMoveDistance : moveDistance;
        Vector3 targetPosition = transform.position + lastInputDirection * distance;
        isMoving = true;
        
        Sequence moveSequence = DOTween.Sequence();
        
        // Move
        moveSequence.Append(
            rb.DOJump(targetPosition, jumpHeight, 1, jumpDuration)
                .SetEase(Ease.OutQuad)
        );
        
        // Rotation
        moveSequence.Join(
            rb.DORotate(
                transform.localEulerAngles + new Vector3(rotationAmount, 0f, 0f),
                jumpDuration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.Linear)
        );
        
        // On complete
        moveSequence.OnComplete(() => {
            isMoving = false;
        });
    }
    
    /// <summary>
    /// Get the current facing direction
    /// </summary>
    public Vector3 GetFacingDirection()
    {
        return lastInputDirection.sqrMagnitude > 0.01f ? lastInputDirection : Vector3.forward;
    }
    
    private void OnDrawGizmos()
    {
        // Draw facing direction
        if (lastInputDirection.sqrMagnitude > 0.01f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, lastInputDirection * 2f);
        }
    }
}
