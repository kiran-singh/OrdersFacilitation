namespace OrdersApi.Services
{
    public interface ICustomDataConverter
    {
        byte[] ToBytes(object data);

        T ToObject<T>(byte[] bytes);
    }
}