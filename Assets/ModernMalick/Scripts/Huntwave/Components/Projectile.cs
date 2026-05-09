using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviourExtended
    {
        [SerializeField] private float speed;
        [SerializeField] private float range;
        
        [Header("Events")]
        [Space(10)]
        public UnityEvent<Collider> onImpact;
        
        [Component] private Rigidbody _rigidbody;
        
        private Vector3 _startPosition;
        private bool _hasImpacted;

        private void OnEnable()
        {
            _hasImpacted = false;
        }

        private void FixedUpdate()
        {
            CheckRange();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTrigger(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTrigger(other);
        }

        private void OnTrigger(Collider other)
        {
            if(_hasImpacted) return;
            _hasImpacted = true;
            onImpact.Invoke(other);
        }

        public void Fire()
        {
            _startPosition = transform.position;
            _rigidbody.linearVelocity = transform.forward * speed;
        }

        private void CheckRange()
        {
            if (Vector3.Distance(_startPosition, transform.position) < range) return;
            Destroy(gameObject);
        }
    }
}