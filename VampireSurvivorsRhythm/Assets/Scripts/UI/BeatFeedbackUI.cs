using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Provides visual feedback for the beat
/// </summary>
public class BeatFeedbackUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image beatOverlay;
    
    [Header("Pulse Settings")]
    [SerializeField] private float pulseIntensity = 0.1f;
    [SerializeField] private float pulseDuration = 0.1f;
    [SerializeField] private Color pulseColor = new Color(1f, 1f, 1f, 0.05f);
    
    [Header("Vignette Settings")]
    [SerializeField] private bool useVignette = true;
    [SerializeField] private float vignetteIntensity = 0.2f;
    
    private Color originalColor;
    
    private void Start()
    {
        // Subscribe to beat events
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.AddListener(OnBeat);
        }
        
        // Setup overlay
        if (beatOverlay != null)
        {
            originalColor = beatOverlay.color;
            
            // Make sure it's transparent initially
            Color transparent = pulseColor;
            transparent.a = 0f;
            beatOverlay.color = transparent;
        }
    }
    
    private void OnDestroy()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.RemoveListener(OnBeat);
        }
    }
    
    /// <summary>
    /// Called on every beat
    /// </summary>
    private void OnBeat()
    {
        if (beatOverlay == null) 
            return;
        
        // Pulse the overlay
        PulseOverlay(); // TERROR
    }
    
    /// <summary>
    /// Pulse the screen overlay
    /// </summary>
    private void PulseOverlay()
    {
        // Kill any existing tweens
        beatOverlay.DOKill();
        
        // Fade in and out
        Sequence pulseSequence = DOTween.Sequence();
        
        pulseSequence.Append(
            beatOverlay.DOColor(pulseColor, pulseDuration * 0.5f)
                .SetEase(Ease.OutQuad)
        );
        
        pulseSequence.Append(
            beatOverlay.DOFade(0f, pulseDuration * 0.5f)
                .SetEase(Ease.InQuad)
        );
    }
    
    /// <summary>
    /// Flash the screen (for damage or special events)
    /// </summary>
    public void FlashScreen(Color flashColor, float duration = 0.2f)
    {
        if (beatOverlay == null) return;
        
        beatOverlay.DOKill();
        
        beatOverlay.DOColor(flashColor, duration * 0.5f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => {
                Color transparent = pulseColor;
                transparent.a = 0f;
                beatOverlay.color = transparent;
            });
    }
}
