using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    public class PopupButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        public void Setup(string label, Action onClick)
        {
            _label.text = label;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}