using System;

namespace OrdersApi.Models
{
    public class EventProcessModel<T> where T : IId
    {
        public Guid CorrelationId => Model?.Id ?? Guid.Empty;
        
        public string EventType { get; set; }

        public T Model { get; set; }
    }
}