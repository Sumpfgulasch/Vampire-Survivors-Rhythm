using UnityEngine;
using DG.Tweening;

/// <summary>
/// Centralized manager for visual and audio feedback
/// </summary>
public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }
    
    [Header("Screen Effects")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float damageShakeIntensity = 0.3f;
    [SerializeField] private float damageShakeDuration = 0.2f;
    
    [Header("Audio")]
    [SerializeField] private bool useAudio = true;
    
    private BeatFeedbackUI beatFeedbackUI;
    private Vector3 originalCameraPosition;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Find main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.localPosition;
        }
        
        // Find beat feedback UI
        beatFeedbackUI = FindObjectOfType<BeatFeedbackUI>();
    }
    
    /// <summary>
    /// Trigger damage feedback
    /// </summary>
    public void TriggerDamageFeedback()
    {
        // Screen shake
        if (mainCamera != null)
        {
            mainCamera.transform.DOShakePosition(damageShakeDuration, damageShakeIntensity)
                .OnComplete(() => {
                    //mainCamera.transform.localPosition = originalCameraPosition;
                });
        }
        
        // Screen flash
        if (beatFeedbackUI != null)
        {
            beatFeedbackUI.FlashScreen(new Color(1f, 0f, 0f, 0.3f), 0.2f);
        }
        
        // Audio feedback
        if (useAudio && AudioManager.Instance != null)
        {
            // Play damage sound via AudioManager
            // AudioManager.Instance.PlaySound("DamageSound");
        }
    }
    
    /// <summary>
    /// Trigger hit feedback (when player hits enemy)
    /// </summary>
    public void TriggerHitFeedback(Vector3 position)
    {
        // Could spawn hit particles here
        // Could play hit sound
        
        if (useAudio && AudioManager.Instance != null)
        {
            // Play hit sound via AudioManager
            // AudioManager.Instance.PlaySound("HitSound");
        }
    }
    
    /// <summary>
    /// Trigger level up feedback
    /// </summary>
    public void TriggerLevelUpFeedback()
    {
        // Screen flash
        if (beatFeedbackUI != null)
        {
            beatFeedbackUI.FlashScreen(new Color(1f, 1f, 0f, 0.5f), 0.5f);
        }
        
        // Audio feedback
        if (useAudio && AudioManager.Instance != null)
        {
            // Play level up sound via AudioManager
            // AudioManager.Instance.PlaySound("LevelUpSound");
        }
    }
    
    /// <summary>
    /// Trigger death feedback
    /// </summary>
    public void TriggerDeathFeedback()
    {
        // Screen shake
        if (mainCamera != null)
        {
            mainCamera.transform.DOShakePosition(0.5f, damageShakeIntensity * 2f);
        }
        
        // Screen flash
        if (beatFeedbackUI != null)
        {
            beatFeedbackUI.FlashScreen(new Color(0f, 0f, 0f, 0.8f), 1f);
        }
        
        // Audio feedback
        if (useAudio && AudioManager.Instance != null)
        {
            // Play death sound via AudioManager
            // AudioManager.Instance.PlaySound("DeathSound");
        }
    }
}
