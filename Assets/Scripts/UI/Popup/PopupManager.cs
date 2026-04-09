using UnityEngine;

namespace UI.Popup
{
    public class PopupManager : MonoBehaviour, IPopupManager
    {
        public static PopupManager Instance { get; private set; }

        [SerializeField] private PopupView _popupPrefab;
        [SerializeField] private Transform _popupContainer;

        private PopupView _activePopup;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Show(IPopupConfig config)
        {
            if (!ValidateSetup())
            {
                return;
            }
            
            Hide();
            _activePopup = Instantiate(_popupPrefab, _popupContainer);
            _activePopup.Setup(config, Hide);
        }

        public void Hide()
        {
            if (!_activePopup) return;
            Destroy(_activePopup.gameObject);
            _activePopup = null;
        }

        private bool ValidateSetup()
        {
            if (!_popupPrefab)
            {
                Debug.LogError("[PopupManager] _popupPrefab is not assigned.", this);
                return false;
            }
            if (!_popupContainer)
            {
                Debug.LogError("[PopupManager] _popupContainer is not assigned.", this);
                return false;
            }
            return true;
        }
    }
}