using ModernMalick.Shooter.Components;
using UnityEngine;

namespace ModernMalick.Shooter.Player.Score
{
    public class ScorePickup : APickup
    {
        [SerializeField] private int worth;
        
        protected override bool TryPickup(GameObject other)
        {
            var score = other.GetComponent<ScoreManager>();
            if(!score) return false;
            score.AddScore(worth);
            return true;
        }
    }
}