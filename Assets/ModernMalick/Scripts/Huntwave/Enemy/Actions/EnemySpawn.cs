using UnityEngine;

namespace ModernMalick.Huntwave.Enemy.Actions
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