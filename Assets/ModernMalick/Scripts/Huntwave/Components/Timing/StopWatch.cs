using System;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Components.Timing
{
    public class StopWatch : MonoBehaviour
    {
        [SerializeField] private bool startOnPlay = true;
        
        [Header("Events")]
        [Space(10)]
        public UnityEvent<TimeSpan> onElapsedChanged;
        
        private bool _running;
        private TimeSpan _elapsed;
        public TimeSpan Elapsed
        {
            get => _elapsed;
            private set
            {
                _elapsed = value;
                onElapsedChanged.Invoke(_elapsed);
            }
        }
        
        private void Start()
        {
            if (!startOnPlay) return;
            StartRunning();
        }

        private void Update()
        {
            if (!_running) return;
            Elapsed += TimeSpan.FromSeconds(Time.deltaTime);
        }

        public void StartRunning()
        {
            _running = true;
        }

        public void StopRunning()
        {
            _running = false;
        }

        public void ResetElapsed()
        {
            Elapsed = TimeSpan.Zero;
        }
    }
}