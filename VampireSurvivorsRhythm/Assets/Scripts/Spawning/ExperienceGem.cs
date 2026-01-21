using UnityEngine;
using DG.Tweening;

/// <summary>
/// Collectible experience gem
/// </summary>
[RequireComponent(typeof(Collider))]
public class ExperienceGem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int experienceValue = 1;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobHeight = 0.2f;
    [SerializeField] private float bobDuration = 1f;
    
    public int ExperienceValue => experienceValue;
    
    private void Start()
    {
        // Rotation animation
        transform.DORotate(new Vector3(0f, 360f, 0f), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        
        // Bob animation
        Vector3 startPos = transform.position;
        transform.DOMoveY(startPos.y + bobHeight, bobDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    
    /// <summary>
    /// Initialize the gem with experience value
    /// </summary>
    public void Initialize(int value)
    {
        experienceValue = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }
    
    /// <summary>
    /// Collect the gem
    /// </summary>
    private void Collect()
    {
        // Add experience to player
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddExperience(experienceValue);
        }
        
        // Collection animation
        transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                Destroy(gameObject);
            });
    }
}
