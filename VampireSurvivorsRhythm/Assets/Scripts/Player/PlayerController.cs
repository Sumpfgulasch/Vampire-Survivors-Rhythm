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
    
    public Vector3 Position => rb.position;
    
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
    
    private Vector2 lastFrameInput = Vector2.zero;
private bool horizontalWasPressedLast = false;

private void Update()
{
    if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive)
    {
        return;
    }
    
    ReadInput();
}

/// <summary>
/// Read input from the Input System
/// </summary>
private void ReadInput()
{
    if (moveAction == null || moveAction.action == null) 
        return;
    
    Vector2 input = moveAction.action.ReadValue<Vector2>();
    
    if (input.sqrMagnitude > 0.01f)
    {
        // Convert to cardinal direction only (no diagonals)
        Vector2 cardinalInput = GetCardinalDirectionByRecency(input);
        
        // Convert 2D input to 3D direction (XZ plane)
        lastInputDirection = new Vector3(cardinalInput.x, 0f, cardinalInput.y);
    }
    
    // Track input for next frame
    lastFrameInput = input;
}

/// <summary>
/// Converts input to cardinal direction based on most recently pressed axis
/// </summary>
private Vector2 GetCardinalDirectionByRecency(Vector2 input)
{
    float absX = Mathf.Abs(input.x);
    float absY = Mathf.Abs(input.y);
    
    bool horizontalPressed = absX > 0.01f;
    bool verticalPressed = absY > 0.01f;
    
    // Check if a new axis was just pressed
    bool horizontalJustPressed = horizontalPressed && Mathf.Abs(lastFrameInput.x) <= 0.01f;
    bool verticalJustPressed = verticalPressed && Mathf.Abs(lastFrameInput.y) <= 0.01f;
    
    // Update which axis was pressed most recently
    if (horizontalJustPressed)
        horizontalWasPressedLast = true;
    else if (verticalJustPressed)
        horizontalWasPressedLast = false;
    
    // If both are pressed, use the most recently pressed one
    if (horizontalPressed && verticalPressed)
    {
        if (horizontalWasPressedLast)
            return new Vector2(Mathf.Sign(input.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(input.y));
    }
    // If only one is pressed, use that one
    else if (horizontalPressed)
    {
        horizontalWasPressedLast = true;
        return new Vector2(Mathf.Sign(input.x), 0f);
    }
    else if (verticalPressed)
    {
        horizontalWasPressedLast = false;
        return new Vector2(0f, Mathf.Sign(input.y));
    }
    
    return Vector2.zero;
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
        Vector3 targetPosition = rb.position + lastInputDirection * distance;
        isMoving = true;
    
        Sequence moveSequence = DOTween.Sequence();
    
        // Move
        moveSequence.Append(
            transform.DOJump(targetPosition, jumpHeight, 1, jumpDuration)
                .SetEase(Ease.OutQuad)
        );
    
        // Rotation - face the movement direction
        if (lastInputDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(lastInputDirection.x, lastInputDirection.z) * Mathf.Rad2Deg;
            moveSequence.Join(
                transform.DORotate(
                    new Vector3(0f, targetAngle, 0f),
                    jumpDuration,
                    RotateMode.Fast
                ).SetEase(Ease.OutQuad)
            );
        }
    
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
