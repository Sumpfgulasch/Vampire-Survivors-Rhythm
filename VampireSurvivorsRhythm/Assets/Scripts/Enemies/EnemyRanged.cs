using UnityEngine;

/// <summary>
/// Enemy that shoots projectiles at the player
/// </summary>
public class EnemyRanged : Enemy
{
    private int shootBeatCounter = 0;
    
    protected override void OnBeat()
    {
        base.OnBeat();
        
        if (isDead) return;
        
        // Handle shooting separately from movement
        shootBeatCounter++;
        
        if (enemyData != null && shootBeatCounter >= enemyData.BeatsToShoot)
        {
            shootBeatCounter = 0;
            ShootProjectile();
        }
    }
    
    /// <summary>
    /// Shoot a projectile at the player
    /// </summary>
    private void ShootProjectile()
    {
        if (Target == null || enemyData == null || enemyData.ProjectilePrefab == null) return;
        
        // Calculate direction to player
        Vector3 direction = (Target.position - transform.position);
        direction.y = 0f;
        direction.Normalize();
        
        // Spawn projectile
        GameObject projectileObj = Instantiate(
            enemyData.ProjectilePrefab,
            transform.position + direction * 0.5f, // Spawn slightly in front
            Quaternion.identity
        );
        
        // Initialize projectile
        EnemyProjectile projectile = projectileObj.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                enemyData.ProjectileDamage,
                enemyData.ProjectileMoveDistance,
                enemyData.ProjectileBeatsToMove
            );
        }
        
        if (showDebug)
        {
            Debug.Log($"{gameObject.name} shot projectile at player");
        }
    }
}
