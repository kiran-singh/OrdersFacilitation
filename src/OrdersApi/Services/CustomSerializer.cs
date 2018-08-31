using Newtonsoft.Json;

namespace OrdersApi.Services
{
    public class CustomSerializer : ICustomSerializer
    {
        public T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public string Serialize<T>(T toLog)
        {
            return JsonConvert.SerializeObject(toLog);
        }

        public string SerializeSecurely(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, 
                new JsonSerializerSettings
                {
                    ContractResolver = new RemoveSensitiveDataContractResolver(),
                });
        }
    }
}