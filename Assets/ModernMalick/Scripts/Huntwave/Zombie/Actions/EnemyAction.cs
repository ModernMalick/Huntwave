using UnityEngine;

namespace ModernMalick.Huntwave.Zombie.Actions
{
    [RequireComponent(typeof(Enemy))]
    public abstract class EnemyAction : MonoBehaviour
    {
        protected Enemy enemy;

        protected virtual void Awake()
        {
            enemy = GetComponent<Enemy>();
        }

        private void OnEnable()
        {
            enemy.onActionPerformed.AddListener(OnActionPerformed);
        }

        private void OnDisable()
        {
            enemy.onActionPerformed.RemoveListener(OnActionPerformed);
        }

        protected abstract void OnActionPerformed();
    }
}