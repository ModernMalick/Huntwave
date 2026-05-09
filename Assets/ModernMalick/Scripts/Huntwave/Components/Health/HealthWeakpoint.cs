using UnityEngine;

namespace ModernMalick.Huntwave.Components.Health
{
    public class HealthWeakpoint : Health
    {
        [Header("Weakpoint")]
        [SerializeField] private Health targetHealth;
        [SerializeField] private int multiplier = 2;

        private void OnEnable()
        {
            onHealthModified.AddListener(MultiplyEffect);
        }

        private void OnDisable()
        {
            onHealthModified.RemoveListener(MultiplyEffect);
        }

        private void MultiplyEffect(int delta)
        {
            targetHealth.ModifyHealth(multiplier * delta);
        }
    }
}