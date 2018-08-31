using System;

namespace OrdersApi.Models
{
    public interface IId
    {
        Guid Id { get; set; }
    }
}