using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Components
{
    public abstract class APickup : MonoBehaviourExtended
    {
        [Header("Events")]
        [Space(10)]
        public UnityEvent onPickedUp;

        private void OnEnable()
        {
            onPickedUp.AddListener(OnPickup);
        }

        private void OnDisable()
        {
            onPickedUp.AddListener(OnPickup);
        }

        private void OnTriggerEnter(Collider other)
        {
            var pickedUp = TryPickup(other.gameObject);
            if (!pickedUp) return;
            onPickedUp.Invoke();
        }

        protected abstract bool TryPickup(GameObject other);

        protected virtual void OnPickup()
        {
            
        }
    }
}