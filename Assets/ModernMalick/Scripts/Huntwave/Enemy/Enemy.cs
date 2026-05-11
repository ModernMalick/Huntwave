using System.Linq;
using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Huntwave.Components;
using ModernMalick.Huntwave.Components.Health;
using ModernMalick.Huntwave.Components.Timing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviourExtended
    {
        [SerializeField] private VisionCone vision;
        
        [Header("Movement")]
        [SerializeField] private bool chase;
        [SerializeField] private float minDistance = 2f;
        
        [Header("Action")]
        [SerializeField] private float actionRange = 2f;
        [SerializeField] private Cooldown actionDelay;
        [SerializeField] private Cooldown actionCooldown;

        [Header("Events")]
        [Space(10)]
        public UnityEvent onTargetDetected;
        public UnityEvent onActionPerformed;
        
        [Component] private NavMeshAgent _agent;
        [Component] private Health _health;

        public Transform Target { get; private set; }

        private bool _isTargetDetected;
        private bool _isPerformingAction;
        public bool IsDead { get; private set; }

        private new void Awake()
        {
            base.Awake();
            Target = GameObject.FindWithTag("Player").transform;
        }

        private void OnEnable()
        {
            _health.onHealthDepleted.AddListener(OnDeath);
        }

        private void OnDisable()
        {
            _health.onHealthDepleted.RemoveListener(OnDeath);
        }

        private void Update()
        {
            if(IsDead) return;
            
            vision.transform.LookAt(Target);
            LookAtTarget();
            
            actionCooldown.Tick(Time.deltaTime);

            if (_isPerformingAction)
            {
                actionDelay.Tick(Time.deltaTime);
                if (actionDelay.IsReady)
                {
                    PerformAction();
                    actionDelay.Reset();
                }
            }
            
            // Debug.Log($"Enemy: {gameObject.name}; Target Visible: {IsTargetVisible()}; Target In Range: {IsInActionRange()}; Action Ready: {actionCooldown.IsReady}; Performing: {_isPerformingAction}");

            if (!_isTargetDetected && IsTargetVisible())
            {
                _isTargetDetected = true;
                onTargetDetected.Invoke();
            }
            
            if (!IsTargetVisible() || _isPerformingAction)
            {
                _agent.isStopped = true;
                return;
            }

            if (IsInActionRange() && IsTargetVisible() && actionCooldown.IsReady)
            {
                StartAction();
            }
            else if (chase && IsOverMinDistance())
            {
                ChaseTarget();
            }
        }

        private void LookAtTarget()
        {
            var targetPosition = Target.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }

        public bool IsTargetVisible()
        {
            return vision.currentHits.Any(hit => hit.collider != null && hit.collider.CompareTag("Player"));
        }

        public bool IsInActionRange()
        {
            return Vector3.Distance(transform.position, Target.position) <= actionRange;
        }
        
        private bool IsOverMinDistance()
        {
            return Vector3.Distance(transform.position, Target.position) > minDistance;
        }

        private void ChaseTarget()
        {
            _agent.isStopped = false;
            _agent.SetDestination(Target.position);
        }

        private void StartAction()
        {
            _isPerformingAction = true;
            actionDelay.Reset();
            _agent.isStopped = true;
        }

        private void PerformAction()
        {
            if(IsDead) return;
            onActionPerformed.Invoke();
            EndAction();
        }

        private void EndAction()
        {
            _isPerformingAction = false;
            actionCooldown.Reset();
        }

        private void OnDeath()
        {
            IsDead = true;
            
            _agent.enabled = false;

            var colliders = GetComponentsInChildren<Collider>(true);
            foreach (var col in colliders)
            {
                Destroy(col);
            }
            
            enabled = false;
        }
    }
}