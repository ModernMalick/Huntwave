using ModernMalick.Huntwave.Components.Health;
using UnityEngine;

namespace ModernMalick.Huntwave.Zombie.Actions
{
    public class EnemyMelee : EnemyAction
    {
        [SerializeField] private int damage;

        protected override void OnActionPerformed()
        {
            if (enemy.IsDead || !enemy.IsInActionRange()) return;
            Health.TryModifyHealth(enemy.Target.gameObject, -damage);
        }
    }
}