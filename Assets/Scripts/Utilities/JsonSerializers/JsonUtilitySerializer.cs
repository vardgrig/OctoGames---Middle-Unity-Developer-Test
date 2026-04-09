using UnityEngine;

namespace Utilities.JsonSerializers
{
    /// <summary>
    /// Fallback serializer using Unity's built-in JsonUtility.
    /// </summary>
    public class JsonUtilitySerializer : IJsonSerializer
    {
        public string Serialize(object data)
        {
            return JsonUtility.ToJson(data, prettyPrint: true);
        }

        public T Deserialize<T>(string json) where T : class
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}