using ModernMalick.Huntwave.Components;
using UnityEngine;

namespace ModernMalick.Huntwave.Enemy.Actions
{
    public class EnemyProjectile : EnemyAction
    {
        [SerializeField] private Transform projectileOrigin;
        [SerializeField] private Projectile projectilePrefab;
        
        protected override void OnActionPerformed()
        {
            if (enemy.IsDead) return;
            var projectile = Instantiate(projectilePrefab, projectileOrigin.position, transform.rotation);
            projectile.transform.LookAt(enemy.Target);
            projectile.Fire();
        }
    }
}