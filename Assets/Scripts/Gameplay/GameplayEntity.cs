using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Base MonoBehaviour for any gameplay entity tracked by EntityRegistry.
    /// Subclass this for interactable objects, story actors, etc.
    /// </summary>
    public abstract class GameplayEntity : MonoBehaviour, IGameplayEntity
    {
        [SerializeField] private string _entityId;

        private bool _isCompleted;

        public string EntityId => string.IsNullOrEmpty(_entityId) ? gameObject.name : _entityId;
        
        public bool IsActive => gameObject.activeInHierarchy && !_isCompleted;

        public event System.Action<IGameplayEntity> OnActiveStateChanged;

        protected virtual void Awake()
        {
            if (EntityRegistry.Instance != null)
            {
                EntityRegistry.Instance.Register(this);
            }
            else
            {
                Debug.LogWarning($"[GameplayEntity] EntityRegistry not found. '{EntityId}' won't be tracked.");
            }
        }

        protected virtual void OnDestroy()
        {
            EntityRegistry.Instance?.Unregister(this);
        }

        // Notify registry consumers when active state changes due to enable/disable
        protected virtual void OnEnable()
        {
            OnActiveStateChanged?.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            OnActiveStateChanged?.Invoke(this);
        }

        /// <summary>
        /// Mark this entity as gameplay-completed (e.g. quest done).
        /// Does not destroy or disable the GameObject.
        /// </summary>
        public void SetCompleted(bool completed)
        {
            if (_isCompleted == completed) return;
            _isCompleted = completed;
            OnActiveStateChanged?.Invoke(this);
        }
    }
}
