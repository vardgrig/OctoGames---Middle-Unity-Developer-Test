using UnityEngine;

namespace Gameplay.Examples
{
    /// <summary>
    /// Example enemy entity.
    /// </summary>
    public class EnemyEntity : GameplayEntity
    {
        [SerializeField] private float _health = 100f;

        public float Health => _health;

        public void TakeDamage(float amount)
        {
            _health -= amount;

            if (_health <= 0f)
            {
                SetCompleted(true);
                Debug.Log($"[EnemyEntity] '{EntityId}' defeated.");
            }
        }
    }
}
