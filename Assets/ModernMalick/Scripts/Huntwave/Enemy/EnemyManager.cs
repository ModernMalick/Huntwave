using System.Collections.Generic;
using ModernMalick.Core.Patterns;
using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Huntwave.Components.Health;
using ModernMalick.Huntwave.Components.Timing;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Enemy
{
    [RequireComponent(typeof(ObjectFactory))]
    public class EnemyManager : MonoBehaviourExtended
    {
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private Cooldown spawnCooldown;
        
        [Component] private ObjectFactory _factory;
        
        private int _currentSpawnIndex;

        private void OnEnable()
        {
            spawnCooldown.onReady.AddListener(SpawnEnemy);
        }

        private void OnDisable()
        {
            spawnCooldown.onReady.RemoveListener(SpawnEnemy);
        }

        private void Update()
        {
            spawnCooldown.Tick(Time.deltaTime);
        }
        
        private void SpawnEnemy()
        {
            spawnCooldown.Reset();
            
            var enemyObject = _factory.Get();
            
            enemyObject.SetActive(false);
            
            var targetPoint = spawnPoints[_currentSpawnIndex];
            enemyObject.transform.SetPositionAndRotation(targetPoint.position, targetPoint.rotation);
            enemyObject.SetActive(true);

            if (enemyObject.TryGetComponent(out Health health))
            {
                health.ResetHealth();

                UnityAction onDepleted = null;
                
                onDepleted = () =>
                {
                    health.onHealthDepleted.RemoveListener(onDepleted);
                    _factory.Release(enemyObject);
                };
                
                health.onHealthDepleted.AddListener(onDepleted);
            }

            _currentSpawnIndex = (_currentSpawnIndex + 1) % spawnPoints.Count;
        }
    }
}