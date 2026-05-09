using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Shooter.Components.Timing;
using Redcode.Core.Redcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace ModernMalick.Shooter.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviourExtended
    {
        [SerializeField] private float actionRange = 1f;
        [SerializeField] private Cooldown actionCooldown;

        [Header("Events")] 
        [Space(10)] 
        public UnityEvent onActionStarted;
        
        [Component] private NavMeshAgent _agent;
        
        public Transform Target { get; private set; }
        
        private bool _isPerformingAction;

        private new void Awake()
        {
            base.Awake();
            Target = GameObject.FindGameObjectWithTag("Player").transform;
            _agent.updateRotation = false;
        }

        private void Update()
        {
            transform.LookAt(Target.position.WithY(transform.position.y));
            
            if (_isPerformingAction) return;
            
            if (IsInActionRange())
            {
                _agent.isStopped = true;
                
                actionCooldown.Tick(Time.deltaTime);

                if (!actionCooldown.IsReady) return;
                
                TryAction();
            }
            else
            {
                actionCooldown.Reset();
                _agent.isStopped = false;
                _agent.SetDestination(Target.position);
            }
        }

        private bool IsInActionRange()
        {
            return Vector3.Distance(transform.position, Target.position) <= actionRange;
        }
        
        private void TryAction()
        {
            _isPerformingAction = true;
            onActionStarted.Invoke();
        }

        public void EndAction()
        {
            _isPerformingAction = false;
            actionCooldown.Reset();
        }
    }
}