using System.Collections.Generic;
using System.Linq;
using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Shooter.Player.Weapons.Guns;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ModernMalick.Shooter.Player.Weapons
{
    public class PlayerArsenal : MonoBehaviourSingleton<PlayerArsenal>
    {
        [Header("Events")]
        [Space(10)]
        public UnityEvent<AWeapon> onSelectedWeaponChanged;
        
        private readonly List<AWeapon> _weapons = new();
        private int _selectedIndex = -1;
        private AWeapon CurrentWeapon => _selectedIndex >= 0 && _selectedIndex < _weapons.Count ? _weapons[_selectedIndex] : null;
        private bool _attackHeld;

        private new void Awake()
        {
            base.Awake();
            for (var i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                if (t.TryGetComponent(out AWeapon weapon))
                    _weapons.Add(weapon);
            }
        }

        private void Start()
        {
            if (_weapons.Count == 0) return;
            SetSlot(0);
        }

        private void Update()
        {
            var weapon = CurrentWeapon;
            if (!_attackHeld || weapon == null) return;

            var gun = weapon as Gun;
            switch (gun != null && gun.IsAutomatic)
            {
                case true when !weapon.CanCharge:
                    weapon.TryAttack();
                    break;
                case false:
                    _attackHeld = false;
                    break;
            }
        }

        public void OnAttack(InputValue value)
        {
            var weapon = CurrentWeapon;
            if (weapon == null) return;

            _attackHeld = value.isPressed;

            if (_attackHeld)
            {
                if (weapon.CanCharge)
                {
                    weapon.StartCharge();
                }
                else
                {
                    weapon.TryAttack();
                }
            }
            else
            {
                if (weapon.CanCharge)
                {
                    weapon.ReleaseCharge();
                }
            }
        }

        public void OnReload(InputValue value)
        {
            var gun = CurrentWeapon as Gun;
            if(gun)
            {
                gun.TryReload();
            }
        }

        public void OnScroll(InputValue value)
        {
            var scroll = value.Get<float>();
            switch (scroll)
            {
                case > 0:
                    ChangeSlot(-1);
                    break;
                case < 0:
                    ChangeSlot(1);
                    break;
            }
        }

        public void OnSelect(InputValue value)
        {
            var index = Mathf.RoundToInt(value.Get<float>()) - 1;
            if (index >= 0 && index < _weapons.Count)
                SetSlot(index);
        }

        private void ChangeSlot(int direction)
        {
            if (_weapons.Count < 2) return;

            var next = (_selectedIndex + direction + _weapons.Count) % _weapons.Count;
            SetSlot(next);
        }

        private void SetSlot(int index)
        {
            if (index == _selectedIndex) return;
            if (index < 0 || index >= _weapons.Count) return;

            var previous = CurrentWeapon;
            if (previous != null)
            {
                previous.gameObject.SetActive(false);
            }

            _selectedIndex = index;
            SelectWeapon();
        }

        private void SelectWeapon()
        {
            var current = CurrentWeapon;
            if (current == null) return;
            current.gameObject.SetActive(true);
            onSelectedWeaponChanged.Invoke(current);
        }

        public void AddWeapon(AWeapon weaponPrefab)
        {
            if (_weapons.Any(weapon => weapon.name == weaponPrefab.name))
            {
                return;
            }
            
            var newWeapon = Instantiate(weaponPrefab, transform);
            newWeapon.name = weaponPrefab.name;
            _weapons.Add(newWeapon);
            
            SetSlot(_weapons.Count - 1);
        }
    }
}