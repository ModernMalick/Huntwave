using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;

namespace ModernMalick.Shooter.Enemy.Actions
{
    [RequireComponent(typeof(Enemy))]
    public abstract class AEnemyAction : MonoBehaviourExtended
    {
        [Component] protected Enemy enemy;
        
        protected virtual void OnEnable()
        {
            enemy.onActionStarted.AddListener(ExecuteAction);
        }

        protected virtual void OnDisable()
        {
            enemy.onActionStarted.RemoveListener(ExecuteAction);
        }

        protected abstract void ExecuteAction();
    }
}