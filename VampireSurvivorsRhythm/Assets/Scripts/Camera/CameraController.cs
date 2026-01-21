using UnityEngine;

/// <summary>
/// Camera controller that follows the player
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float fixedHeight = 10f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -5f);
    
    [Header("Rotation")]
    [SerializeField] private Vector3 lookAtOffset = Vector3.zero;
    
    private void Start()
    {
        // Find player if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate target position (follow X and Z, fixed Y)
        Vector3 targetPos = new Vector3(
            target.position.x + offset.x,
            fixedHeight,
            target.position.z + offset.z
        );
        
        // Smoothly move camera
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
        
        // Look at player with offset
        Vector3 lookAtPos = target.position + lookAtOffset;
        transform.LookAt(lookAtPos);
    }
    
    /// <summary>
    /// Set the target to follow
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    /// <summary>
    /// Set the camera height
    /// </summary>
    public void SetHeight(float height)
    {
        fixedHeight = height;
    }
}
