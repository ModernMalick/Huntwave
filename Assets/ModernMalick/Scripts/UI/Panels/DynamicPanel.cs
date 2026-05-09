using Core.LeanTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModernMalick.UI.Panels
{
    public class DynamicPanel : MonoBehaviour
    {
        [SerializeField] private Button selectedButton;

        [Header("Events")]
        [Space(10)]
        public UnityEvent onOpened;
        public UnityEvent onClosed;
        
        public void Open()
        {
            transform.localScale = Vector3.zero;
            LeanTween.scale(gameObject, Vector3.one, UITweener.Instance.tweenTime)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    selectedButton.Select();
                    onOpened.Invoke();
                });
        }

        public void Close()
        {
            LeanTween.scale(gameObject, Vector3.zero, UITweener.Instance.tweenTime)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    onClosed.Invoke();
                    gameObject.SetActive(false);
                });
        }

        public void SelectButton()
        {
            selectedButton.Select();
        }
    }
}