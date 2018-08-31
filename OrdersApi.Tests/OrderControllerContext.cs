using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Castle.Core.Internal;
using EventStore.ClientAPI;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrdersApi.Models;
using OrdersApi.Services;

namespace OrdersApi.Tests
{
    public class OrderControllerContext
    {
        private readonly Fixture _fixture = new Fixture();

        private AddOrderModel _addOrderModel;
        private Mock<IEventStoreConnection> _eventStoreConnection;
        private IConfigurationRoot _configuration;
        private HttpResponseMessage _response;
        private Mock<ICustomerService> _customerService;

        public void Valid_order_request_is_received()
        {
            _configuration = ConfigHelper.GetConfiguration();
            _addOrderModel = _fixture.Create<AddOrderModel>();
            _addOrderModel.Email = $"{_fixture.Create<string>()}@gmail.com";
        }

        public async Task Order_is_submitted()
        {
            _eventStoreConnection = new Mock<IEventStoreConnection>();
            
            _customerService = new Mock<ICustomerService>();
            _customerService.Setup(x => x.Get(_addOrderModel.Email))
                .Returns(Task.FromResult(new Customer {Id = Guid.NewGuid()}));

            _eventStoreConnection.Setup(x => x.AppendToStreamAsync("ORDER_STORING_STREAM", ExpectedVersion.Any,
                It.Is<EventData>(y =>
                    y.EventId != Guid.Empty && y.Type == "AddOrder" && !y.IsJson && y.Data.Length > 0 &&
                    y.Metadata.IsNullOrEmpty())))
                .ReturnsAsync(new WriteResult());

            var testServer = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                    {
                        services.AddSingleton<IEventStoreConnection>(_eventStoreConnection.Object);
                        services.AddTransient<ICustomerService>(_ => _customerService.Object);
                    })
                .UseConfiguration(_configuration)
                .UseStartup<Startup>());

            var client = testServer.CreateClient();

            _response = await client.PostAsJsonAsync("/api/order", _addOrderModel);
        }

        public void Order_accepted_response_is_returned()
        {
            _customerService.Verify(x => x.Get(_addOrderModel.Email));

            _eventStoreConnection.Verify(x => x.AppendToStreamAsync("ORDER_STORING_STREAM", ExpectedVersion.Any,
                It.Is<EventData>(y =>
                    y.EventId != Guid.Empty && y.Type == "AddOrder" && !y.IsJson && y.Data.Length > 0 &&
                    y.Metadata.IsNullOrEmpty())));
            
            _response.Should().NotBeNull();
            _response.StatusCode.Should().Be(HttpStatusCode.Created);
            
        }

        public void Model_has_email_set_to(string email)
        {
            Valid_order_request_is_received();
            _addOrderModel.Email = email;
        }

        public void Then_event_data_is_Not_actioned_on_correct_event_stream()
        {
            _eventStoreConnection.Verify(x => x.AppendToStreamAsync("ORDER_STORING_STREAM", ExpectedVersion.Any,
                It.Is<EventData>(y =>
                    y.EventId != Guid.Empty && y.Type == "AddOrder" && !y.IsJson && y.Data.Length > 0 &&
                    y.Metadata.IsNullOrEmpty())), Times.Never);
        }

        public void Then_bad_request_is_returned_with_value(string errorMessage)
        {
            _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var response = _response.Content.ReadAsStringAsync().Result;
            response.Should().Contain(errorMessage);
        }
    }
}