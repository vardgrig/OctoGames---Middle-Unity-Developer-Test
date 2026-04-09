using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    [DefaultExecutionOrder(-100)]
    public class SaveLoadBootstrap : MonoBehaviour // Simple Bootstrapper to apply SaveLoadConfigSO settings to SaveLoadUtility
    {
        [SerializeField] private SaveLoadConfigSO _config;

        [Header("Scene Transition (optional)")]
        [Tooltip("Name of the scene to load after configuration. Leave empty to stay on this scene.")]
        [SerializeField] private string _nextSceneName;

        private void Awake()
        {
            if (_config == null)
            {
                Debug.LogError("[SaveLoadBootstrap] No SaveLoadConfigSO assigned — " +
                               "SaveLoadUtility.Default will use unsafe placeholder defaults.", this);
            }
            else
            {
                _config.ApplyTo(SaveLoadUtility.Default);

                Debug.Log($"[SaveLoadBootstrap] Configured SaveLoadUtility.Default " +
                          $"(dir: '{_config.SaveDirectoryName}', " +
                          $"encryption: {_config.UseEncryption}, " +
                          $"serializer: {_config.Serializer})");
            }

            if (!string.IsNullOrWhiteSpace(_nextSceneName))
            {
                SceneManager.LoadScene(_nextSceneName);
            }
        }
    }
}
