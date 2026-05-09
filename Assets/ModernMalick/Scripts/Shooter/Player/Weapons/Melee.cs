using ModernMalick.Shooter.Components.Health;
using UnityEngine;

namespace ModernMalick.Shooter.Player.Weapons
{
    public class MeleeWeapon : AWeapon
    {
        [Header("Melee")]
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private bool instantHit;

        protected override void ExecuteAttack()
        {
            if(instantHit) CompleteAttack();
        }

        public void CompleteAttack()
        {
            var rayOrigin = playerCamera.transform.position;
            var direction = playerCamera.transform.forward;

            Physics.SphereCast(
                rayOrigin,
                radius,
                direction,
                out var hit,
                range,
                mask
            );

            if (hit.collider != null)
            {
                Health.TryModifyHealth(hit.collider.gameObject, -currentDamage);
                onHit.Invoke(hit);
            }
            
            base.ExecuteAttack();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            
            var targetPosition = transform.position + transform.forward * range;
            
            Gizmos.DrawWireSphere(targetPosition, radius);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}