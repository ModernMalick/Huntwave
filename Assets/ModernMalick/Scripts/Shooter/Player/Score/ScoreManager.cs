using ModernMalick.Core.Patterns;
using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Shooter.Player.Score
{
    [RequireComponent(typeof(ObjectFactory))]
    public class ScoreManager : MonoBehaviourExtended
    {
        [SerializeField] private bool higherIsBetter = true;
        [SerializeField] private string playerPrefsKey = "HighScore";

        [Header("Events")] [Space(10)] 
        public UnityEvent<int> onInitialBestSet;
        public UnityEvent<int> onScoreChanged;
        public UnityEvent<int> onNewHighScore;

        [Component] private ObjectFactory _factory;
        
        public int CurrentScore
        {
            get => _currentScore;
            set
            {
                _currentScore = value;
                onScoreChanged.Invoke(_currentScore);
            }
        }

        public int InitialBest
        {
            get => _initialBest;
            set
            {
                _initialBest = value;
                if(_initialBest == 0) return;
                onInitialBestSet.Invoke(value);
            }
        }
        
        private int _currentScore;
        private int _initialBest;

        private new void Awake()
        {
            base.Awake();
            InitialBest = PlayerPrefs.GetInt(playerPrefsKey, 0);
        }
        
        public void AddScore(int score)
        {
            CurrentScore += score;
            TryUpdateHighScore();
        }

        public void SpawnScoreObject(Vector3 position)
        {
            var scoreObject = _factory.Get();
            scoreObject.transform.position = position;
        }
        
        public void TryUpdateHighScore()
        {
            if (!IsNewRecord(CurrentScore)) return;
            PlayerPrefs.SetInt(playerPrefsKey, CurrentScore);
            if(InitialBest == 0) return;
            onNewHighScore.Invoke(CurrentScore);
        }

        private bool IsNewRecord(int current)
        {
            if (InitialBest == 0) return true;
            return higherIsBetter ? current > InitialBest : current < InitialBest;
        }
    }
}