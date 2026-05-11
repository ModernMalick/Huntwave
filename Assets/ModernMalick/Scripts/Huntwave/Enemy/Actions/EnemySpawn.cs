using ModernMalick.Huntwave.Enemy.Actions;
using UnityEngine;

namespace ModernMalick.Common.Enemies.Actions
{
    public class EnemySpawn : EnemyAction
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject spawnPrefab;
        
        protected override void OnActionPerformed()
        {
            Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}