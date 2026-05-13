using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;

namespace ModernMalick.Huntwave.Player.Weapons.Guns
{
    [RequireComponent(typeof(Animator))]
    public class GunAnimator : MonoBehaviourExtended
    {
        [SerializeField] private float referenceClipLength;
        
        [Component] private Animator _animator;
        [ParentComponent] private Gun _gun;
        
        private static readonly int SHOOT = Animator.StringToHash("Shoot");
        private static readonly int RELOAD = Animator.StringToHash("Reload");
        private static readonly int SHOT_SPEED = Animator.StringToHash("Shot Speed");
        private static readonly int RELOAD_SPEED = Animator.StringToHash("Reload Speed");
        
        private new void Awake()
        {
            base.Awake();
            
            var shotMultiplier = referenceClipLength * _gun.AttackRate;
            var reloadMultiplier = referenceClipLength / _gun.ReloadCooldown.Duration;

            _animator.SetFloat(SHOT_SPEED, shotMultiplier);
            _animator.SetFloat(RELOAD_SPEED, reloadMultiplier);
            
            _animator.keepAnimatorStateOnDisable = true;
        }

        private void OnEnable()
        {
            _gun.onAttack.AddListener(OnShotFired);
            _gun.ReloadCooldown.onStarted.AddListener(OnReloadStarted);
        }

        private void OnDisable()
        {
            _gun.onAttack.RemoveListener(OnShotFired);
            _gun.ReloadCooldown.onStarted.RemoveListener(OnReloadStarted);
        }

        private void OnShotFired()
        {
            _animator.SetTrigger(SHOOT);
        }

        private void OnReloadStarted()
        {
            _animator.SetTrigger(RELOAD);
        }
    }
}