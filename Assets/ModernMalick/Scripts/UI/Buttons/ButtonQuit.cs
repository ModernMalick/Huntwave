using UnityEngine;

namespace ModernMalick.UI.Buttons
{
    public class ButtonQuit : AButton
    {
        protected override void OnClick()
        {
            Application.Quit();
        }
    }
}