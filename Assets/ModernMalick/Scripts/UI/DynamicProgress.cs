using Core.LeanTween;
using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace ModernMalick.UI
{
    [RequireComponent(typeof(Image))]
    public class DynamicProgress : MonoBehaviourExtended
    {
        [SerializeField] protected float tweenDuration;
        [SerializeField] private bool tweenMinMax;
        [SerializeField] private Image image;

        public void UpdateProgress(float progress)
        {
            if(!image) return;
            
            LeanTween.cancel(gameObject);
            
            var diff = Mathf.Abs(image.fillAmount - progress);
            var calculatedDuration = diff * tweenDuration;
            
            LeanTween.value(gameObject, image.fillAmount, progress, calculatedDuration)
                .setOnUpdate(val => 
                {
                    image.fillAmount = val;
                })
                .setOnComplete(() => 
                {
                    if (tweenMinMax && progress is 0 or >= 1)
                    {
                        UITweener.Instance.ValueChangeTween(gameObject);
                    }
                });
        }
    }
}