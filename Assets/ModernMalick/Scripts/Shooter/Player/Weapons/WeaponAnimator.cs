using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Shooter.Player.Weapons.Guns;
using UnityEngine;

namespace ModernMalick.Shooter.Player.Weapons
{
    [RequireComponent(typeof(Animator))]
    public class WeaponAnimator : MonoBehaviourExtended
    {
        [ParentComponent] private Gun _gun;
        [Component] private Animator _animator;
        
        private static readonly int ATTACK = Animator.StringToHash("Attack");
        
        private static readonly int CHARGE = Animator.StringToHash("Charge");
        private static readonly int IS_CHARGED = Animator.StringToHash("IsCharged");

        private new void Awake()
        {
            base.Awake();
            _animator.keepAnimatorStateOnDisable = true;
        }
        
        private void OnEnable()
        {
            _gun.onAttack.AddListener(OnAttacked);
            _gun.onChargeChanged.AddListener(OnChargeChanged);
        }

        private void OnDisable()
        {
            _gun.onAttack.RemoveListener(OnAttacked);
            _gun.onChargeChanged.RemoveListener(OnChargeChanged);
        }

        private void Update()
        {
            _animator.SetBool(IS_CHARGED, _gun.IsCharged);
        }

        private void OnAttacked()
        {
            _animator.SetTrigger(ATTACK);
            _animator.ResetTrigger(CHARGE);
        }

        private void OnChargeChanged(bool chargeStarted)
        {
            if(!chargeStarted) return;
            _animator.SetTrigger(CHARGE);
        }
    }
}