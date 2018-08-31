using System;

namespace OrdersApi.Models
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
    }
}