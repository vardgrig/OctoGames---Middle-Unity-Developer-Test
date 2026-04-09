using UnityEngine;
using Utilities.JsonSerializers;

namespace Utilities
{
    public enum SerializerType
    {
        Newtonsoft,
        JsonUtility
    }

    [CreateAssetMenu(fileName = "SaveLoadConfig", menuName = "Utilities/Save Load Config")]
    public class SaveLoadConfigSO : ScriptableObject
    {
        public string SaveDirectoryName = "saves";
        public bool UseEncryption = true;
        public string EncryptionKey = "enc-key";
        public SerializerType Serializer = SerializerType.Newtonsoft;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(SaveDirectoryName))
            {
                Debug.LogWarning($"[SaveLoadConfig] '{name}': SaveDirectoryName is empty — will fall back to 'saves'.");
                SaveDirectoryName = "saves";
            }

            if (!UseEncryption)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(EncryptionKey))
            {
                Debug.LogWarning(
                    $"[SaveLoadConfig] '{name}': EncryptionKey is empty — obfuscation will not work correctly.");
            }
        }

        public void ApplyTo(SaveLoadUtility utility)
        {
            utility.SaveDirectoryName = string.IsNullOrWhiteSpace(SaveDirectoryName) ? "saves" : SaveDirectoryName;
            utility.Configure(EncryptionKey, UseEncryption);
            utility.SetSerializer(BuildSerializer());
        }

        private IJsonSerializer BuildSerializer() => Serializer switch
        {
            SerializerType.JsonUtility => new JsonUtilitySerializer(),
            _ => new NewtonsoftJsonSerializer()
        };
    }
}