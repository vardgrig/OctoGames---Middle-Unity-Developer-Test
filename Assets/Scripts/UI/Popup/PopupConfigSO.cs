using UnityEngine;

namespace UI.Popup
{
    [CreateAssetMenu(fileName = "NewPopupConfig", menuName = "UI/Popup Config")]
    public class PopupConfigSO : ScriptableObject, IPopupConfig
    {
        public string Title;
        
        [TextArea(2, 6)] 
        public string Body;
        
        public PopupButtonData[] Buttons;

        string IPopupConfig.Title => Title;
        string IPopupConfig.Body => Body;
        PopupButtonConfig[] IPopupConfig.Buttons
        {
            get
            {
                var result = new PopupButtonConfig[Buttons?.Length ?? 0];
                for (var i = 0; i < result.Length; i++)
                {
                    var d = Buttons[i];
                    result[i] = new PopupButtonConfig(d.Label, d.OnClick.Invoke, d.CloseOnClick);
                }
                return result;
            }
        }

        private void OnValidate()
        {
            if (Buttons is { Length: < 1 or > 5 })
            {
                Debug.LogWarning(
                    $"[PopupConfigSO] '{name}': Buttons must have 1–5 entries (found {Buttons.Length}).");
            }
        }
    }
}