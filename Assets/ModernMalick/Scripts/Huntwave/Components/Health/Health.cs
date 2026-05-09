using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Components.Health
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;

        [Header("Events")]
        [Space(10)]
        public UnityEvent<int> onHealthChanged;
        public UnityEvent<int> onMaxHealthChanged;
        public UnityEvent<int> onHealthModified;
        public UnityEvent<float> onHealthPercentageChanged;
        public UnityEvent<int> onHealthIncreased;
        public UnityEvent<int> onHealthDecreased;
        public UnityEvent onHealthDepleted;

        private int MaxHealth
        {
            get => maxHealth;
            set
            {
                maxHealth = value;
                onMaxHealthChanged.Invoke(maxHealth);
            }
        }
        
        private int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (currentHealth <= 0 && value <= 0) return;
                
                var clamped = Mathf.Clamp(value, 0, MaxHealth);
                var delta = clamped - currentHealth;

                currentHealth = clamped;
                onHealthChanged.Invoke(currentHealth);
                onHealthPercentageChanged.Invoke(currentHealth / (float)MaxHealth);

                switch (delta)
                {
                    case > 0:
                        onHealthIncreased.Invoke(delta);
                        break;
                    case < 0:
                        onHealthDecreased.Invoke(-delta);
                        break;
                }

                if (delta == 0 || currentHealth != 0) return;
                onHealthDepleted.Invoke();
            }
        }

        private void Start()
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }

        public void ResetHealth()
        {
            CurrentHealth = MaxHealth;
        }

        public void ModifyHealth(int delta)
        {
            CurrentHealth += delta;
            onHealthModified.Invoke(delta);
        }

        public static bool TryModifyHealth(GameObject other, int delta)
        {
            var health = other.GetComponent<Health>();
            if(!health) return false;
            health.ModifyHealth(delta);
            return true;
        }

        public bool IsHealthFull()
        {
            return CurrentHealth == MaxHealth;
        }
    }
}