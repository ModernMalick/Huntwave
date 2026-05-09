using System;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Shooter.Components.Timing
{
    [Serializable]
    public class Cooldown
    {
        [SerializeField] private float duration = 1f;

        [Header("Events")]
        [Space(10)]
        public UnityEvent onStarted;
        public UnityEvent onReady;
        public UnityEvent<float> onElapsedChanged;

        private float _elapsed;

        public float Elapsed
        {
            get => _elapsed;
            set
            {
                _elapsed = Mathf.Clamp(value, 0, duration);
                onElapsedChanged.Invoke(Mathf.Clamp01(_elapsed / duration));

                if (_elapsed >= duration)
                {
                    onReady.Invoke();
                }
            }
        }
        
        public bool IsReady => _elapsed >= duration;

        public void Reset()
        {
            Elapsed = 0f;
        }

        public void Refill()
        {
            Elapsed = duration;
        }

        public void Tick(float deltaTime)
        {
            if (IsReady) return;

            if (Elapsed == 0)
            {
                onStarted.Invoke();
            }
            
            Elapsed += deltaTime;
        }
    }
}