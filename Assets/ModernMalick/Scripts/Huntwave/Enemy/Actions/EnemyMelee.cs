using ModernMalick.Huntwave.Components.Health;
using UnityEngine;

namespace ModernMalick.Huntwave.Enemy.Actions
{
    public class EnemyMelee : EnemyAction
    {
        [SerializeField] private int damage;

        protected override void OnActionPerformed()
        {
            if (enemy.IsDead || !enemy.IsTargetVisible()) return;
            Health.TryModifyHealth(enemy.Target.gameObject, -damage);
        }
    }
}