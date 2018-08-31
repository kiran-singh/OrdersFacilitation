using System.Threading.Tasks;
using OrdersApi.Models;

namespace OrdersApi.Services
{
    public interface ICustomerService
    {
        Task<Customer> Get(string email);
    }
}