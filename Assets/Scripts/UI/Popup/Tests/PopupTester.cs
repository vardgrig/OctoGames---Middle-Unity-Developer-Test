using UnityEngine;

namespace UI.Popup.Tests
{
    public class PopupTester : MonoBehaviour
    {
        [Header("Designer-driven (optional)")]
        [SerializeField] private PopupConfigSO _soConfig;

        private void OnGUI()
        {
            float w = 260f, h = 50f, x = 20f, startY = 20f, gap = 60f;

            if (GUI.Button(new Rect(x, startY, w, h), "Show 1-Button Popup"))
            {
                ShowOneButton();
            }

            if (GUI.Button(new Rect(x, startY + gap, w, h), "Show 3-Button Popup"))
            {
                ShowThreeButtons();
            }

            if (GUI.Button(new Rect(x, startY + gap * 2, w, h), "Show 5-Button Popup"))
            {
                ShowFiveButtons();
            }

            if (_soConfig != null && GUI.Button(new Rect(x, startY + gap * 3, w, h), "Show SO Popup"))
            {
                PopupManager.Instance.Show(_soConfig);
            }
        }

        private void ShowOneButton()
        {
            PopupManager.Instance.Show(new PopupConfig
            {
                Title = "Notice",
                Body  = "This is a single-button popup.",
                Buttons = new[]
                {
                    new PopupButtonConfig("OK", () => Debug.Log("[PopupTester] OK clicked"))
                }
            });
        }

        private void ShowThreeButtons()
        {
            PopupManager.Instance.Show(new PopupConfig
            {
                Title = "Confirm",
                Body  = "Would you like to proceed?",
                Buttons = new[]
                {
                    new PopupButtonConfig("Yes",    () => Debug.Log("[PopupTester] Yes clicked")),
                    new PopupButtonConfig("No",     () => Debug.Log("[PopupTester] No clicked")),
                    new PopupButtonConfig("Cancel", () => Debug.Log("[PopupTester] Cancel clicked"), closeOnClick: true)
                }
            });
        }

        private void ShowFiveButtons()
        {
            PopupManager.Instance.Show(new PopupConfig
            {
                Title = "Rate Us",
                Body  = "How would you rate your experience?",
                Buttons = new[]
                {
                    new PopupButtonConfig("1star", () => Debug.Log("[PopupTester] 1 star")),
                    new PopupButtonConfig("2stars", () => Debug.Log("[PopupTester] 2 stars")),
                    new PopupButtonConfig("3stars", () => Debug.Log("[PopupTester] 3 stars")),
                    new PopupButtonConfig("4stars", () => Debug.Log("[PopupTester] 4 stars")),
                    new PopupButtonConfig("5stars", () => Debug.Log("[PopupTester] 5 stars"))
                }
            });
        }
    }
}
