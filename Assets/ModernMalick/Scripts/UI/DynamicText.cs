using System;
using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using TMPro;
using UnityEngine;

namespace ModernMalick.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DynamicText : MonoBehaviourExtended
    {
        [SerializeField] private string format;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        [SerializeField] private bool animate = true;

        [Component] private TextMeshProUGUI _textMesh;
        
        public void UpdateText(string value)
        {
            SetText(value);
        }

        public void UpdateText(int value)
        {
            SetText(value.ToString(format));
        }

        public void UpdateText(float value)
        {
            SetText(value.ToString(format));
        }

        public void UpdateText(TimeSpan value)
        {
            SetText(value.ToString(format));
        }

        private void SetText(string value)
        {
            if (_textMesh == null)
            {
                _textMesh = GetComponent<TextMeshProUGUI>();
            }
            
            _textMesh.text = $"{prefix}{value}{suffix}";
            if (animate)
            {
                UITweener.Instance.ValueChangeTween(gameObject);
            }
        }
    }
}