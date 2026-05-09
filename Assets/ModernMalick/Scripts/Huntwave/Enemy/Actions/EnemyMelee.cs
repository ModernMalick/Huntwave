using ModernMalick.Huntwave.Components.Health;
using UnityEngine;

namespace ModernMalick.Huntwave.Enemy.Actions
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyMelee : AEnemyAction
    {
        [SerializeField] private int damage;

        protected override void ExecuteAction()
        {
            Health.TryModifyHealth(enemy.Target.gameObject, -damage);
            enemy.EndAction();
        }
    }
}