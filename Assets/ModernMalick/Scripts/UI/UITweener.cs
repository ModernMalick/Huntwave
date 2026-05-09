using Core.LeanTween;
using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;

namespace ModernMalick.UI
{
    public class UITweener : MonoBehaviourSingleton<UITweener>
    {
        [Header("Tween")] 
        [SerializeField] private float tweenMaxScale = 1.1f;
        [SerializeField] public float tweenTime = 0.25f;
        
        public void ValueChangeTween(GameObject tweenTarget)
        {
            LeanTween.scale(tweenTarget, tweenMaxScale * Vector3.one, tweenTime)
                .setOnComplete(() =>
                {
                    LeanTween.scale(tweenTarget, Vector3.one, tweenTime);
                });
        }
    }
}