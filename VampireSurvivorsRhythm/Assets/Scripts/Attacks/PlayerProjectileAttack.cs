using System.Linq;
using UnityEngine;

/// <summary>
/// Projectile attack implementation
/// </summary>
public class PlayerProjectileAttack : PlayerAttackInstance {
    protected override void ExecuteAttack() {
        if (player == null || attackData == null) return;

        ProjectileAttackSO projectileData = attackData as ProjectileAttackSO;
        if (projectileData == null || projectileData.VisualPrefab == null) {
            Debug.LogWarning("ProjectileAttack: Invalid projectile data or missing prefab");
            return;
        }
        
        var closestEnemy = EnemySpawner.Instance.AliveEnemies
            .OrderBy(e => Vector3.Distance(e.transform.position, player.position)).FirstOrDefault();
        
        // Don't shoot if no enemy close
        if (closestEnemy == null) 
            return;

        // instantiate
        Vector3 direction = (closestEnemy.transform.position - player.position).normalized;
        var pos = player.position + direction * 0.5f;
        GameObject projObj = Instantiate(projectileData.VisualPrefab, pos, Quaternion.identity);
        
        PlayerProjectile proj = projObj.GetComponent<PlayerProjectile>();
        if (proj == null) {
            proj = projObj.AddComponent<PlayerProjectile>();
        }

        // Initialize projectile
        proj.Initialize(direction, projectileData);
    }
}