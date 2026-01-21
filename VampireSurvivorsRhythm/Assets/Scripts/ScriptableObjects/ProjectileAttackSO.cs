using UnityEngine;

/// <summary>
/// Projectile attack configuration
/// </summary>
[CreateAssetMenu(fileName = "ProjectileAttack", menuName = "Rhythm Game/Attacks/Projectile Attack")]
public class ProjectileAttackSO : AttackTypeSO
{
    [Header("Projectile Movement")]
    [Tooltip("Distance the projectile moves per movement action")]
    public float MoveDistance = 1f;
    
    [Tooltip("How many beats between projectile movements")]
    public float BeatsToMove = 0.5f;
    
    [Header("Projectile Behavior")]
    [Tooltip("Destroy projectile on hit")]
    public bool DestroyOnHit = true;
    
    [Tooltip("Number of enemies the projectile can pierce through (0 = destroy on first hit)")]
    public int MaxPierceCount = 0;
    
    [Tooltip("Lifetime of projectile in seconds (0 = infinite)")]
    public float Lifetime = 5f;
    
    private void OnEnable()
    {
        Category = AttackCategory.Projectile;
    }
}
