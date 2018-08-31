using System.Text;
using Newtonsoft.Json;

namespace OrdersApi.Services
{
    public class CustomDataConverter : ICustomDataConverter
    {
        public byte[] ToBytes(object data)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }

        public T ToObject<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }
    }
}