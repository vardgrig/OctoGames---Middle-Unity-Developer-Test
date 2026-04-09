using Newtonsoft.Json;

namespace Utilities.JsonSerializers
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
        };

        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, Settings);
        }

        public T Deserialize<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}