using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;
using OrdersApi.Services;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IEventService _eventService;

        public OrderController(ICustomerService customerService, IEventService eventService)
        {
            _eventService = eventService;
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AddOrderModel model)
        {
            var customer = await _customerService.Get(model.Email);
            
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                InventoryItems = model.InventoryItems,
            };
            
            await _eventService.ProcessEventAsync(new EventProcessModel<Order>
            {
                EventType = $"Add{nameof(Order)}",
                Model = order,
            });
            
            return StatusCode((int) HttpStatusCode.Created);
        }
    }
}