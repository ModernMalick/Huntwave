using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.UI.Panels
{
    public class PanelManager : MonoBehaviour
    {
        [Header("Events")]
        [Space(10)]
        public UnityEvent onPanelOpened;
        public UnityEvent onPanelClosed;
        public UnityEvent onUIOpened;
        public UnityEvent onUIClosed;

        private DynamicPanel _previousPanel;
        private List<DynamicPanel> _openPanels;

        private void Awake()
        {
            _openPanels = new List<DynamicPanel>();
        }

        public void OpenPanel(DynamicPanel panel)
        {
            if(panel.isActiveAndEnabled) return;
            panel.gameObject.SetActive(true);
            panel.Open();
            
            onPanelOpened.Invoke();
            
            if (_openPanels.Count == 0)
            {
                onUIOpened.Invoke();
            }
            
            _openPanels.Add(panel);
        }

        public void ClosePanel(DynamicPanel panel)
        {
            if(!panel.isActiveAndEnabled) return;
            panel.Close();
            _previousPanel = panel;
            
            onPanelClosed.Invoke();
            
            _openPanels.Remove(panel);
            
            if (_openPanels.Count == 0)
            {
                onUIClosed.Invoke();
            }
        }

        public void TogglePanel(DynamicPanel panel)
        {
            if (panel.isActiveAndEnabled)
            {
                ClosePanel(panel);
            }
            else
            {
                OpenPanel(panel);
            }
        }

        public void SelectPreviousPanel()
        {
            if(!_previousPanel) return;
            _previousPanel.SelectButton();
        }
    }
}