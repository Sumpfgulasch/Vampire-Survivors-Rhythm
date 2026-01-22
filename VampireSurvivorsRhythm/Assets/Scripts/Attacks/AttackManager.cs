using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all active player attacks
/// </summary>
public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance { get; private set; }
    
    [Header("Configuration")]
    [SerializeField] private Transform player;
    [SerializeField] private List<AttackTypeSO> startingAttacks = new List<AttackTypeSO>();
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = false;
    
    private List<PlayerAttackInstance> activeAttacks = new List<PlayerAttackInstance>();
    
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
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Add starting attacks
        foreach (var attack in startingAttacks)
        {
            AddAttack(attack);
        }
    }
    
    /// <summary>
    /// Add a new attack to the player
    /// </summary>
    public void AddAttack(AttackTypeSO attackData)
    {
        if (attackData == null)
        {
            Debug.LogWarning("AttackManager: Cannot add null attack");
            return;
        }
        
        // Create attack GameObject
        GameObject attackObj = new GameObject($"Attack_{attackData.AttackName}");
        attackObj.transform.SetParent(transform);
        
        // Add appropriate attack component based on category
        PlayerAttackInstance attack = null;
        
        switch (attackData.Category)
        {
            case AttackCategory.Projectile:
                attack = attackObj.AddComponent<PlayerProjectileAttack>();
                break;
                
            // Add other attack types here when implemented
            case AttackCategory.Beam:
                Debug.LogWarning("AttackManager: Beam attacks not yet implemented");
                break;
                
            case AttackCategory.Shield:
                Debug.LogWarning("AttackManager: Shield attacks not yet implemented");
                break;
                
            case AttackCategory.Bomb:
                Debug.LogWarning("AttackManager: Bomb attacks not yet implemented");
                break;
                
            case AttackCategory.AutoAttacker:
                Debug.LogWarning("AttackManager: AutoAttacker attacks not yet implemented");
                break;
        }
        
        if (attack != null)
        {
            attack.Initialize(attackData, player);
            activeAttacks.Add(attack);
            
            if (showDebug)
            {
                Debug.Log($"AttackManager: Added attack {attackData.AttackName}");
            }
        }
        else
        {
            Destroy(attackObj);
        }
    }
    
    /// <summary>
    /// Apply an upgrade to an attack
    /// </summary>
    public void ApplyUpgrade(AttackUpgradeSO upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogWarning("AttackManager: Cannot apply null upgrade");
            return;
        }
        
        // Find the target attack
        PlayerAttackInstance targetAttack = null;
        
        if (upgrade.TargetAttack != null)
        {
            // Specific attack upgrade
            foreach (var attack in activeAttacks)
            {
                // Check if this attack matches the target
                // This is a simplified check - you might want to add an ID system
                if (attack.GetType().Name.Contains(upgrade.TargetAttack.Category.ToString()))
                {
                    targetAttack = attack;
                    break;
                }
            }
        }
        else
        {
            // Global upgrade - apply to all attacks
            Debug.Log("AttackManager: Global upgrade not yet implemented");
        }
        
        if (targetAttack != null)
        {
            // Apply the upgrade based on type
            ApplyUpgradeToAttack(targetAttack, upgrade);
            
            if (showDebug)
            {
                Debug.Log($"AttackManager: Applied upgrade {upgrade.UpgradeName}");
            }
        }
        else
        {
            Debug.LogWarning($"AttackManager: Could not find target attack for upgrade {upgrade.UpgradeName}");
        }
    }
    
    /// <summary>
    /// Apply upgrade modifications to an attack
    /// </summary>
    private void ApplyUpgradeToAttack(PlayerAttackInstance attack, AttackUpgradeSO upgrade)
    {
        // This is a simplified implementation
        // In a full implementation, you'd modify the attack's properties based on upgrade type
        
        switch (upgrade.Type)
        {
            case UpgradeType.IncreaseDamage:
                // Increase damage
                Debug.Log($"Increased damage by {upgrade.ValueModifier}");
                break;
                
            case UpgradeType.IncreaseSpeed:
                // Decrease beats to spawn
                Debug.Log($"Increased attack speed");
                break;
                
            case UpgradeType.IncreaseRange:
                // Increase move distance
                Debug.Log($"Increased range");
                break;
                
            case UpgradeType.IncreasePierce:
                // Increase pierce count
                Debug.Log($"Increased pierce");
                break;
                
            case UpgradeType.ProjectileSalvo:
                // Enable salvo mode
                Debug.Log($"Enabled projectile salvo");
                break;
                
            case UpgradeType.IncreaseProjectileSpeed:
                // Decrease projectile beats to move
                Debug.Log($"Increased projectile speed");
                break;
        }
    }
    
    /// <summary>
    /// Get all active attacks
    /// </summary>
    public List<PlayerAttackInstance> GetActiveAttacks()
    {
        return new List<PlayerAttackInstance>(activeAttacks);
    }
    
    /// <summary>
    /// Get count of active attacks
    /// </summary>
    public int GetActiveAttackCount()
    {
        return activeAttacks.Count;
    }
}
