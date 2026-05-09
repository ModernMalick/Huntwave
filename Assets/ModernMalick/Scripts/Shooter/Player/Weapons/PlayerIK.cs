using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Shooter.Player.Weapons.Guns;
using UnityEngine;

namespace ModernMalick.Shooter.Player.Weapons
{
    [RequireComponent(typeof(Animator))]
    public class PlayerIK : MonoBehaviourExtended
    {
        [Component] private Animator _animator;

        private Transform _rightGrip;
        private Transform _leftGrip;

        public void SetWeapon(AWeapon weapon)
        {
            _rightGrip = weapon.RightGrip;
            _leftGrip = weapon.LeftGrip;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_rightGrip)
            {
                SetHandIK(AvatarIKGoal.RightHand, _rightGrip);
            }
            else
            {
                ClearHandIK(AvatarIKGoal.RightHand);
            }

            if (_leftGrip)
            {
                SetHandIK(AvatarIKGoal.LeftHand, _leftGrip);
            }
            else
            {
                ClearHandIK(AvatarIKGoal.LeftHand);
            }
        }

        private void SetHandIK(AvatarIKGoal goal, Transform target)
        {
            _animator.SetIKPositionWeight(goal, 1f);
            _animator.SetIKRotationWeight(goal, 1f);

            _animator.SetIKPosition(goal, target.position);
            _animator.SetIKRotation(goal, target.rotation);
        }

        private void ClearHandIK(AvatarIKGoal goal)
        {
            _animator.SetIKPositionWeight(goal, 0f);
            _animator.SetIKRotationWeight(goal, 0f);
        }
    }
}