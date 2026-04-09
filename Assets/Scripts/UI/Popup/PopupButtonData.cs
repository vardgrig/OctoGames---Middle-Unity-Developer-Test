using System;
using UnityEngine.Events;

namespace UI.Popup
{
    [Serializable]
    public class PopupButtonData
    {
        public string Label;
        public UnityEvent OnClick;
        public bool CloseOnClick = true;
    }
}