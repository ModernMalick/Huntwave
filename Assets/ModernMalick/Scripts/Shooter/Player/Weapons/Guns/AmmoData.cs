using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Shooter.Player.Weapons.Guns
{
    [CreateAssetMenu(menuName = "MM/Ammo")]
    public class AmmoData : ScriptableObject
    {
        public bool infiniteReserve;
        public int startingReserve;
        
        [Header("Events")]
        [Space(10)]
        public UnityEvent<int> onReserveChanged;
    
        private int _currentReserve;
        public int CurrentReserve
        {
            get => _currentReserve;
            private set
            {
                _currentReserve = value;
                onReserveChanged.Invoke(value);
            }
        }

        public void OnEnable()
        {
            CurrentReserve = startingReserve;
        }

        public bool HasReserve()
        {
            return infiniteReserve || CurrentReserve > 0;
        }

        public int PullAmmo(int amount)
        {
            if (infiniteReserve) return amount;
            
            var available = Mathf.Min(amount, CurrentReserve);
            CurrentReserve -= available;
            
            return available;
        }

        public void AddAmmo(int amount)
        {
            CurrentReserve += amount;
        }
    }
}