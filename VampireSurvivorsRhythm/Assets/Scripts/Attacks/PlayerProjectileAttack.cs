using UnityEngine;

/// <summary>
/// Projectile attack implementation
/// </summary>
public class PlayerProjectileAttack : AttackInstance
{
    protected override void ExecuteAttack()
    {
        if (player == null || attackData == null) return;
        
        ProjectileAttackSO projectileData = attackData as ProjectileAttackSO;
        if (projectileData == null || projectileData.VisualPrefab == null)
        {
            Debug.LogWarning("ProjectileAttack: Invalid projectile data or missing prefab");
            return;
        }
        
        // Get player facing direction
        PlayerController playerController = player.GetComponent<PlayerController>();
        Vector3 direction = playerController != null ? playerController.GetFacingDirection() : Vector3.forward;
        
        // Spawn projectile at player position
        var pos = player.position + direction * 0.5f;
        GameObject projObj = Instantiate(projectileData.VisualPrefab, pos, Quaternion.identity);
        
        // Add PlayerProjectile component if not already present
        PlayerProjectile proj = projObj.GetComponent<PlayerProjectile>();
        if (proj == null)
        {
            proj = projObj.AddComponent<PlayerProjectile>();
        }
        
        // Initialize projectile
        proj.Initialize(direction, projectileData);
    }
}
