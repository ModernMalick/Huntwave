using UnityEngine;
using UnityEngine.InputSystem;

namespace ModernMalick.Huntwave.Game
{
    public class PlayerPause : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        
        public void OnPause(InputValue inputValue)
        {
            gameManager.TogglePause();
        }
    }
}