namespace OrdersApi.Services
{
    public interface ICustomSerializer
    {
        T Deserialize<T>(string content);
        
        string Serialize<T>(T toLog);
        
        string SerializeSecurely(object value);
    }
}