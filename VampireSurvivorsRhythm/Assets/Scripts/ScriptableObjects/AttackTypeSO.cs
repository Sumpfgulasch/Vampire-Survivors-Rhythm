using UnityEngine;

/// <summary>
/// Base class for all attack types
/// </summary>
public abstract class AttackTypeSO : ScriptableObject
{
    [Header("Identity")]
    public string AttackName = "Attack";
    public Sprite Icon;
    public GameObject VisualPrefab;
    
    [Header("Timing")]
    [Tooltip("How many beats between attack spawns")]
    public int BeatsToSpawn = 2;
    
    [Header("Combat")]
    [Tooltip("Base damage of the attack")]
    public float Damage = 1f;
    
    [Header("Category")]
    public AttackCategory Category;
    
    [Header("Upgrade Tracking")]
    [Tooltip("Current upgrade level (tracked at runtime)")]
    [System.NonSerialized]
    public int UpgradeLevel = 0;
}

/// <summary>
/// Categories of attacks
/// </summary>
public enum AttackCategory
{
    Projectile,
    Beam,
    Shield,
    Bomb,
    AutoAttacker
}
