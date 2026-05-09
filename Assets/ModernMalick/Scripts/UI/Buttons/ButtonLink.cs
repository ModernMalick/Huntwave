using UnityEngine;

namespace ModernMalick.UI.Buttons
{
    public class ButtonLink : AButton
    {
        [SerializeField] private string link;
        
        protected override void OnClick()
        {
            Application.OpenURL(link);
        }
    }
}