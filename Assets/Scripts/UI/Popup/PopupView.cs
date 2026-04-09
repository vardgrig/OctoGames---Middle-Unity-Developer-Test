using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Popup
{
    public class PopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private Transform _buttonsContainer;
        [SerializeField] private PopupButtonView _buttonPrefab;

        private readonly List<PopupButtonView> _spawnedButtons = new();

        public void Setup(IPopupConfig config, Action onClose)
        {
            _title.text = config.Title;
            _body.text = config.Body;

            ClearButtons();

            var buttons = config.Buttons;
            if (buttons == null || buttons.Length == 0)
            {
                Debug.LogWarning("[PopupView] IPopupConfig has no buttons configured.");
                return;
            }

            int count = Mathf.Clamp(buttons.Length, 1, 5);
            for (var i = 0; i < count; i++)
            {
                var btn = buttons[i];
                SpawnButton(btn.Label, () =>
                {
                    btn.OnClick?.Invoke();
                    if (btn.CloseOnClick) onClose?.Invoke();
                });
            }
        }

        private void SpawnButton(string label, Action onClick)
        {
            var view = Instantiate(_buttonPrefab, _buttonsContainer);
            view.Setup(label, onClick);
            _spawnedButtons.Add(view);
        }

        private void ClearButtons()
        {
            foreach (var btn in _spawnedButtons)
                Destroy(btn.gameObject);
            _spawnedButtons.Clear();
        }
    }
}