using System.Threading.Tasks;
using MongoDB.Driver;
using OrdersApi.Models;

namespace OrdersApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMongoCollection<Customer> _mongoCollection;

        public CustomerService(IMongoCollection<Customer> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }


        public async Task<Customer> Get(string email)
        {
            return await _mongoCollection.Find(x => x.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        }
    }
}