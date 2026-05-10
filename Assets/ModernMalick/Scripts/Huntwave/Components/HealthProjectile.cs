using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;

namespace ModernMalick.Huntwave.Components
{
    [RequireComponent(typeof(Projectile))]
    public class HealthProjectile : MonoBehaviourExtended
    {
        [SerializeField] private int healthDelta;
        
        [Component] private Projectile _projectile;

        private void OnEnable()
        {
            _projectile.onImpact.AddListener(ApplyHealthDelta);
        }

        private void OnDisable()
        {
            _projectile.onImpact.RemoveListener(ApplyHealthDelta);
        }

        private void ApplyHealthDelta(Collider other)
        {
            Health.Health.TryModifyHealth(other.gameObject, healthDelta);
        }
    }
}