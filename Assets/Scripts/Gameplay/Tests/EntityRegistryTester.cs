using System.Collections.Generic;
using Gameplay.Examples;
using UnityEngine;

namespace Gameplay.Tests
{
    public class EntityRegistryTester : MonoBehaviour
    {
        private readonly List<EnemyEntity> _spawnedEnemies = new();
        private int _spawnCounter;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 20, 340, 600));
            GUILayout.Label("=== Entity Registry Tester ===");
            GUILayout.Space(8);

            if (GUILayout.Button("Spawn Enemy"))
            {
                SpawnEnemy();
            }

            GUILayout.Space(4);

            if (GUILayout.Button("Disable Last Enemy (GameObject off)"))
            {
                OperateOnLast(e => e.gameObject.SetActive(false));
            }

            if (GUILayout.Button("Re-enable Last Enemy"))
            {
                OperateOnLast(e => e.gameObject.SetActive(true));
            }

            if (GUILayout.Button("Complete Last Enemy (SetCompleted)"))
            {
                OperateOnLast(e => e.SetCompleted(true));
            }

            if (GUILayout.Button("Damage Last Enemy (100 dmg → auto-complete)"))
            {
                OperateOnLast(e => e.TakeDamage(100f));
            }

            if (GUILayout.Button("Destroy Last Enemy"))
            {
                OperateOnLast(e =>
                {
                    _spawnedEnemies.Remove(e);
                    Destroy(e.gameObject);
                });
            }

            GUILayout.Space(8);

            if (GUILayout.Button("Log Active Entities"))
            {
                LogActive();
            }

            if (GUILayout.Button("Log All Entities (incl. inactive)"))
            {
                LogAll();
            }

            GUILayout.Space(8);

            if (GUILayout.Button("Spawn 5 Enemies"))
            {
                for (var i = 0; i < 5; i++)
                {
                    SpawnEnemy();
                }
            }

            if (GUILayout.Button("Clear All (Destroy All)"))
            {
                ClearAll();
            }

            GUILayout.Space(12);
            GUILayout.Label(GetStatusText());

            GUILayout.EndArea();
        }

        private void SpawnEnemy()
        {
            if (!EntityRegistry.Instance)
            {
                Debug.LogError("[Tester] EntityRegistry not found in scene.");
                return;
            }

            var go = new GameObject($"Enemy_{++_spawnCounter}");
            go.transform.position = Random.insideUnitSphere * 3f;
            var enemy = go.AddComponent<EnemyEntity>();
            _spawnedEnemies.Add(enemy);

            Debug.Log($"[Tester] Spawned '{go.name}'");
        }

        private void OperateOnLast(System.Action<EnemyEntity> action)
        {
            if (_spawnedEnemies.Count == 0)
            {
                Debug.LogWarning("[Tester] No enemies spawned yet.");
                return;
            }

            for (var i = _spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if (_spawnedEnemies[i])
                {
                    action(_spawnedEnemies[i]);
                    return;
                }
            }

            Debug.LogWarning("[Tester] All tracked enemies have been destroyed.");
        }

        private void LogActive()
        {
            var active = EntityRegistry.Instance.GetActive();
            Debug.Log($"[Tester] Active entities ({active.Count}):");
            foreach (var e in active)
            {
                Debug.Log($"  → {e.EntityId}");
            }
        }

        private void LogAll()
        {
            var all = EntityRegistry.Instance.GetAll();
            Debug.Log($"[Tester] All registered entities ({all.Count}):");
            foreach (var e in all)
            {
                Debug.Log($"  → {e.EntityId}  |  IsActive: {e.IsActive}");
            }
        }

        private void ClearAll()
        {
            foreach (var e in _spawnedEnemies)
            {
                if (e)
                {
                    Destroy(e.gameObject);
                }
            }

            _spawnedEnemies.Clear();
            Debug.Log("[Tester] All enemies destroyed.");
        }

        private string GetStatusText()
        {
            if (!EntityRegistry.Instance)
            {
                return "EntityRegistry: NOT FOUND";
            }

            var all = EntityRegistry.Instance.GetAll().Count;
            var active = EntityRegistry.Instance.GetActive().Count;
            return $"Registered: {all}  |  Active: {active}";
        }
    }
}
