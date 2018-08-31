using System;

namespace OrdersApi.Models
{
    public class AddOrderModel
    {
        public string Email { get; set; }
        
        public InventoryItem[] InventoryItems { get; set; }
    }
}