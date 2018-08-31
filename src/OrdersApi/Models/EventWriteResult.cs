using EventStore.ClientAPI;

namespace OrdersApi.Models
{
    public class EventWriteResult
    {
        public string Error { get; set; }
    }
}