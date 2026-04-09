using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Central registry for all gameplay entities in the scene.
    /// </summary>
    public class EntityRegistry : MonoBehaviour // We can use DI (Zenject) to avoid the singleton pattern if desired
    {
        public static EntityRegistry Instance { get; private set; }

        //  O(1) complexity
        private readonly HashSet<IGameplayEntity> _entities = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Register(IGameplayEntity entity)
        {
            if (entity == null)
            {
                return;
            }
            _entities.Add(entity);
        }

        public void Unregister(IGameplayEntity entity)
        {
            if (entity == null)
            {
                return;
            }
            _entities.Remove(entity);
        }

        /// <summary>
        /// Returns a snapshot of all currently active entities.
        /// </summary>
        public IReadOnlyList<IGameplayEntity> GetActive()
        {
            var result = new List<IGameplayEntity>(_entities.Count);

            foreach (var entity in _entities)
            {
                if (entity.IsActive)
                {
                    result.Add(entity);
                }
            }

            return result;
        }

        /// <summary>Returns all registered entities </summary>
        public IReadOnlyList<IGameplayEntity> GetAll()
        {
            return new List<IGameplayEntity>(_entities);
        }

        /// <summary>Find a specific entity by its ID. </summary>
        public IGameplayEntity GetById(string id)
        {
            foreach (var entity in _entities)
            {
                if (entity.EntityId == id)
                {
                    return entity;
                }
            }

            return null;
        }
    }
}
