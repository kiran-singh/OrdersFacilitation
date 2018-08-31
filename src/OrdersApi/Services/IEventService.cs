using System.Threading.Tasks;
using OrdersApi.Models;

namespace OrdersApi.Services
{
    public interface IEventService
    {
        Task<EventWriteResult> ProcessEventAsync<T>(EventProcessModel<T> eventProcessModel) where T : IId;
    }
}