using System;

namespace OrdersApi.Models
{
    public class InventoryItem
    {
        public Guid Guid { get; set; }

        public int Quantity { get; set; }
    }
}