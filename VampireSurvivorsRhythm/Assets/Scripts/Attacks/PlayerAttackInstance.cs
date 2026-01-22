using UnityEngine;

/// <summary>
/// Base class for all attack instances
/// </summary>
public abstract class PlayerAttackInstance : MonoBehaviour
{
    protected AttackTypeSO attackData;
    protected Transform player;
    protected int beatCounter = 0;
    
    /// <summary>
    /// Initialize the attack with data and player reference
    /// </summary>
    public virtual void Initialize(AttackTypeSO data, Transform playerTransform)
    {
        attackData = data;
        player = playerTransform;
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
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive)
        {
            return;
        }
        
        if (attackData == null) return;
        
        beatCounter++;
        
        if (beatCounter >= attackData.BeatsToSpawn)
        {
            beatCounter = 0;
            ExecuteAttack();
        }
    }
    
    /// <summary>
    /// Execute the attack (override in derived classes)
    /// </summary>
    protected abstract void ExecuteAttack();
}
