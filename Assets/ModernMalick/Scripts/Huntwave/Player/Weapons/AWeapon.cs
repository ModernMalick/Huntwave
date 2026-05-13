using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Player.Weapons
{
    public abstract class AWeapon : MonoBehaviour
    {
        [Header("Mechanics")]
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float attackRate = 1f;
        
        [Header("Environment")]
        [SerializeField] protected LayerMask mask;
        [SerializeField] protected float range = 100;
        
        [Header("Charge")] 
        [SerializeField] protected bool canCharge;
        [SerializeField] protected float chargeTime = 1f;
        [SerializeField] protected int damageMultiplier = 2;

        [Header("IK")]
        [SerializeField] protected Transform leftGrip;
        [SerializeField] protected Transform rightGrip;
        
        [Header("Events")]
        [Space(10)]
        public UnityEvent onAttack;
        public UnityEvent<RaycastHit> onHit;
        public UnityEvent<bool> onChargeChanged;
        public UnityEvent<float> onChargeUpdated;
        public UnityEvent onChargeCompleted;

        public float AttackRate => attackRate;
        public bool IsCharged { get; private set; }
        public bool CanCharge => canCharge;
        public Transform LeftGrip => leftGrip;
        public Transform RightGrip => rightGrip;
        
        protected Camera playerCamera;
        protected int currentDamage;
        
        private bool initialized;
        private float lastAttackTime;
        private float chargeStartTime;
        private Coroutine chargeRoutine;
        
        private void OnEnable()
        {
            if (initialized) return;
            initialized = true;
            playerCamera = Camera.main;
            ResetDamage();
            lastAttackTime = Time.time - 1 / attackRate;
        }
        
        private void ResetDamage()
        {
            currentDamage = damage;
        }
        
        protected bool CanAttack()
        {
            return Time.time > lastAttackTime + 1f / attackRate;
        }

        public virtual void TryAttack()
        {
            if (!CanAttack()) return;
            
            ExecuteAttack();
            lastAttackTime = Time.time;
        }

        protected virtual void ExecuteAttack()
        {
            lastAttackTime = Time.time;
            onAttack.Invoke();
            IsCharged = false;
            ResetDamage();
        }
        
        public void StartCharge()
        {
            if (!CanAttack()) return;
            
            IsCharged = false;
            
            StopChargeRoutine();
            chargeRoutine = StartCoroutine(ChargeSequence());
            onChargeChanged?.Invoke(true);
        }

        public void ReleaseCharge()
        {
            if(chargeRoutine == null) return;
            
            StopChargeRoutine();
            onChargeUpdated.Invoke(0f);
            onChargeChanged.Invoke(false);
            
            TryAttack();
        }
        
        private IEnumerator ChargeSequence()
        {
            chargeStartTime = Time.time;
            var progress = 0f;

            while (progress < 1f)
            {
                progress = (Time.time - chargeStartTime) / chargeTime;
                onChargeUpdated.Invoke(Mathf.Clamp01(progress));
                yield return null;
            }
            
            currentDamage = damage * damageMultiplier;

            IsCharged = true;
            onChargeUpdated.Invoke(1f);
            onChargeCompleted.Invoke();
        }

        private void StopChargeRoutine()
        {
            if (chargeRoutine == null) return;
            StopCoroutine(chargeRoutine);
            chargeRoutine = null;
        }
    }
}