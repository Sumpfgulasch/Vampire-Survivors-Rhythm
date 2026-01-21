using UnityEngine;

/// <summary>
/// Projectile attack implementation
/// </summary>
public class ProjectileAttack : AttackInstance
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
        GameObject projObj = Instantiate(projectileData.VisualPrefab, player.position, Quaternion.identity);
        
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
