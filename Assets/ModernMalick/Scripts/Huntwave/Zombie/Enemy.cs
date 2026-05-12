using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Huntwave.Components.Health;
using ModernMalick.Huntwave.Components.Timing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Zombie
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviourExtended
    {
        [Header("Action")]
        [SerializeField] private float actionRange = 2f;
        [SerializeField] private Cooldown actionCooldown;

        [Header("Animation")]
        [SerializeField] private float velocitySmoothing = 0.1f;
        
        [Header("Events")]
        [Space(10)]
        public UnityEvent onActionPerformed;
        
        [Component] private NavMeshAgent _agent;
        [Component] private Animator _animator;
        [Component] private Health _health;

        public Transform Target { get; private set; }

        private static readonly int ACTION = Animator.StringToHash("Action");
        private static readonly int VELOCITY = Animator.StringToHash("Velocity");
        private static readonly int HURT = Animator.StringToHash("Hurt");
        private static readonly int DEATH = Animator.StringToHash("Death");

        private bool _isPerformingAction;
        public bool IsDead { get; private set; }
        private float _currentVelocity; 

        private new void Awake()
        {
            base.Awake();
            Target = GameObject.FindWithTag("Player").transform;
        }

        private void OnEnable()
        {
            _health.onHealthDecreased.AddListener(OnHurt);
            _health.onHealthDepleted.AddListener(OnDeath);
        }

        private void OnDisable()
        {
            _health.onHealthDecreased.RemoveListener(OnHurt);
            _health.onHealthDepleted.RemoveListener(OnDeath);
        }

        private void Update()
        {
            if(IsDead) return;
            
            if (_agent.isStopped)
            {
                LookAtTarget();
            }
            
            actionCooldown.Tick(Time.deltaTime);
            
            if(_isPerformingAction)
            {
                if(_agent.isOnNavMesh)
                {
                    _agent.isStopped = true;
                }
                return;
            }

            if (IsInActionRange() && HasLineOfSight() && actionCooldown.IsReady)
            {
                StartAction();
            }
            else if (!IsInActionRange() || !HasLineOfSight()) {
                ChaseTarget();
            }
        }

        private void FixedUpdate()
        {
            var targetVelocity = _agent.velocity.magnitude / _agent.speed;
            var smoothVelocity = Mathf.SmoothDamp(_animator.GetFloat(VELOCITY), targetVelocity, ref _currentVelocity, velocitySmoothing);
            _animator.SetFloat(VELOCITY, smoothVelocity);
        }
        
        public bool IsInActionRange()
        {
            return Vector3.Distance(transform.position, Target.position) <= actionRange;
        }

        public bool HasLineOfSight()
        {
            var direction = Target.position - transform.position;
            
            if (Physics.Raycast(transform.position, direction, out var hit, actionRange))
            {
                return hit.transform == Target;
            }

            return true;
        }

        private void ChaseTarget()
        {
            if (!_agent.isOnNavMesh) return;
            
            var path = new NavMeshPath();
            
            if (!_agent.CalculatePath(Target.position, path)) return;
            if (path.status != NavMeshPathStatus.PathComplete) return;
            
            _agent.isStopped = false;
            _agent.SetDestination(Target.position);
        }

        private void StartAction()
        {
            _animator.SetTrigger(ACTION);
            _isPerformingAction = true;
        }

        private void PerformAction()
        {
            if(IsDead) return;
            onActionPerformed.Invoke();
        }

        private void EndAction()
        {
            _isPerformingAction = false;
            actionCooldown.Reset();
        }

        private void OnHurt(int _)
        {
            _animator.SetTrigger(HURT);
        }

        private void OnDeath()
        {
            IsDead = true;
            
            _agent.enabled = false;
            _animator.SetTrigger(DEATH);
            
            enabled = false;
        }
        
        private void LookAtTarget()
        {
            var targetPosition = Target.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }
    }
}