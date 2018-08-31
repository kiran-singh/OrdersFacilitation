using System;

namespace OrdersApi.Models
{
    public class Order : IId
    {
        public Guid Id { get; set; }
        
        public Guid CustomerId { get; set; }
        
        public InventoryItem[] InventoryItems { get; set; }

    }
}