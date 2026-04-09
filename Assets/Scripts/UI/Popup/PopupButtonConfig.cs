using System;

namespace UI.Popup
{
    public readonly struct PopupButtonConfig
    {
        public readonly string Label;
        public readonly Action OnClick;
        public readonly bool CloseOnClick;

        public PopupButtonConfig(string label, Action onClick = null, bool closeOnClick = true)
        {
            Label = label;
            OnClick = onClick;
            CloseOnClick = closeOnClick;
        }
    }
}