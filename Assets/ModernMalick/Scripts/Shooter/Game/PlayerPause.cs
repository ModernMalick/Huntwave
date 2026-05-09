using UnityEngine;
using UnityEngine.InputSystem;

namespace ModernMalick.Shooter.Game
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