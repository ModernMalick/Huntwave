using ModernMalick.Core.Patterns;
using ModernMalick.Huntwave.Components;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Enemy.Actions
{
    [RequireComponent(typeof(ObjectFactory))]
    public class EnemyProjectile : AEnemyAction
    {
        [SerializeField] private Transform projectileOrigin;
        
        [Component] private ObjectFactory _projectileFactory;
        
        protected override void ExecuteAction()
        {
            var projectileObject = _projectileFactory.Get();
            
            var projectile = projectileObject.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                projectile.transform.position = projectileOrigin.position;
                projectile.transform.LookAt(enemy.Target);

                UnityAction<Collider> onImpact = null;

                onImpact = _ =>
                {
                    projectile.onImpact.RemoveListener(onImpact);
                    _projectileFactory.Release(projectileObject);
                };
                
                projectile.onImpact.AddListener(onImpact);
                
                projectile.Fire();
            }
            
            enemy.EndAction();
        }
    }
}