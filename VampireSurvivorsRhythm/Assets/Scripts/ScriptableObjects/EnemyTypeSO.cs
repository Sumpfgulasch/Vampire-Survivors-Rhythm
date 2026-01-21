using UnityEngine;

/// <summary>
/// Defines the type and behavior of an enemy
/// </summary>
[CreateAssetMenu(fileName = "EnemyType", menuName = "Rhythm Game/Enemy Type")]
public class EnemyTypeSO : ScriptableObject
{
    [Header("Identity")]
    public string EnemyName = "Enemy";
    public GameObject Prefab;
    
    [Header("Movement")]
    [Tooltip("Distance the enemy moves per movement action")]
    public float MoveDistance = 0.5f;
    
    [Tooltip("How many beats between movements (e.g., 1 = every beat, 2 = every other beat)")]
    public int BeatsToMove = 1;
    
    [Header("Combat")]
    [Tooltip("Damage dealt on collision with player")]
    public float CollisionDamage = 0.5f;
    
    [Tooltip("Health of the enemy")]
    public float Health = 1f;
    
    [Tooltip("Experience value when killed")]
    public int ExperienceValue = 1;
    
    [Header("Behavior")]
    [Tooltip("Type of behavior this enemy exhibits")]
    public EnemyBehaviorType BehaviorType = EnemyBehaviorType.Chase;
    
    [Header("Ranged Settings (if applicable)")]
    [Tooltip("Projectile to shoot (for ranged enemies)")]
    public GameObject ProjectilePrefab;
    
    [Tooltip("How many beats between shots")]
    public int BeatsToShoot = 4;
    
    [Tooltip("Projectile damage")]
    public float ProjectileDamage = 0.5f;
    
    [Tooltip("Projectile move distance per beat")]
    public float ProjectileMoveDistance = 1f;
    
    [Tooltip("How many beats between projectile movements")]
    public float ProjectileBeatsToMove = 0.5f;
}

/// <summary>
/// Types of enemy behaviors
/// </summary>
public enum EnemyBehaviorType
{
    Chase,      // Moves toward player
    Stationary, // Doesn't move
    Ranged      // Shoots projectiles at player
}
