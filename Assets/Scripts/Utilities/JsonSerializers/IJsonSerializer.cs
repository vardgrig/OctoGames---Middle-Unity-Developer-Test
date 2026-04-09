namespace Utilities.JsonSerializers
{
    public interface IJsonSerializer
    {
        string Serialize(object data);
        T Deserialize<T>(string json) where T : class;
    }
}

