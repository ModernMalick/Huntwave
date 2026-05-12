using System;
using System.Collections.Generic;
using Redcode.Core.Redcode;
using UnityEngine;

namespace ModernMalick.Huntwave.Zombie
{
    [Serializable]
    public class EnemySpawn
    {
        public Transform transformPoint;
        public GameObject enemyPrefab;
    }
    
    [RequireComponent(typeof(BoxCollider))]
    public class EnemySpawnTrigger : MonoBehaviour
    {
        [SerializeField] private List<EnemySpawn> spawnPoints;

        private bool _activated;
        
        private void OnTriggerEnter(Collider other)
        {
            if(_activated || !other.CompareTag("Player")) return;
            
            _activated = true;
            
            foreach (var enemySpawn in spawnPoints)
            {
                var zombie = Instantiate(enemySpawn.enemyPrefab, enemySpawn.transformPoint.position, transform.rotation);
                zombie.transform.LookAt(transform.position.WithY(enemySpawn.transformPoint.position.y));
            }
        }
    }
}