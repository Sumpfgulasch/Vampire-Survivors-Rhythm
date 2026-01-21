using UnityEngine;

/// <summary>
/// Defines an upgrade for an attack
/// </summary>
[CreateAssetMenu(fileName = "AttackUpgrade", menuName = "Rhythm Game/Attack Upgrade")]
public class AttackUpgradeSO : ScriptableObject
{
    [Header("Identity")]
    public string UpgradeName = "Upgrade";
    public Sprite Icon;
    
    [TextArea(3, 5)]
    public string Description = "Upgrade description";
    
    [Header("Target")]
    [Tooltip("Which attack this upgrade applies to (null = applies to all)")]
    public AttackTypeSO TargetAttack;
    
    [Header("Upgrade Properties")]
    public UpgradeType Type;
    
    [Tooltip("Modifier value (additive or multiplicative depending on type)")]
    public float ValueModifier = 1f;
    
    [Header("Special Upgrades")]
    [Tooltip("For projectile salvo upgrade")]
    public int SalvoCount = 3;
    
    [Tooltip("Angle between salvo projectiles")]
    public float SalvoAngleSpread = 15f;
}

/// <summary>
/// Types of upgrades available
/// </summary>
public enum UpgradeType
{
    IncreaseDamage,        // Increase damage
    IncreaseSpeed,         // Decrease BeatsToSpawn (faster attacks)
    IncreaseRange,         // Increase MoveDistance
    IncreasePierce,        // Increase MaxPierceCount
    ProjectileSalvo,       // Fire multiple projectiles at once
    IncreaseProjectileSpeed // Decrease BeatsToMove for projectiles
}
