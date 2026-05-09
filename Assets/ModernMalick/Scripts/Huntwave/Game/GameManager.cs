using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Events")] 
        [Space(10)]
        public UnityEvent onPauseToggled;
        public UnityEvent<bool> onGameEnded;
        
        private bool _isPausePanelOpen;

        private void Start()
        {
            SetPaused(false);
        }

        public void TogglePause()
        {
            onPauseToggled.Invoke();
        }

        public void EndGame(bool isWin)
        {
            onGameEnded.Invoke(isWin);
            PlayerPrefs.Save();
        }

        public void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0 : 1;
        }
    }
}