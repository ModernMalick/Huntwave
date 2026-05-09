using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.UI.Panels;
using UnityEngine;

namespace ModernMalick.UI
{
    [RequireComponent(typeof(PanelManager))]
    public class CursorManager : MonoBehaviourExtended
    {
        [SerializeField] private bool startHidden = true;

        private void Start()
        {
            if (startHidden)
            {
                HideCursor();
            }
            else
            {
                ShowCursor();
            }
        }

        public void ShowCursor()
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }

        public void HideCursor()
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }
}