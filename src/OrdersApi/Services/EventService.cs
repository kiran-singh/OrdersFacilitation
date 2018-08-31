using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using OrdersApi.Models;
using OrdersApiÍ;
using ILogger = Serilog.ILogger;

namespace OrdersApi.Services
{
    public class EventService : IEventService
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger _logger;
        private readonly ICustomDataConverter _customDataConverter;
        private readonly string _streamName;
        private readonly ICustomSerializer _customSerializer;

        public EventService(ILogger logger, IEventStoreConnection eventStoreConnection,
            ICustomDataConverter customDataConverter, string streamName, ICustomSerializer customSerializer)
        {
            _eventStoreConnection = eventStoreConnection;
            _customDataConverter = customDataConverter;
            _streamName = streamName;
            _customSerializer = customSerializer;
            _logger = logger;
        }

        public async Task<EventWriteResult> ProcessEventAsync<T>(EventProcessModel<T> eventProcessModel) where T : IId
        {
            if (eventProcessModel == null || eventProcessModel.Model == null)
                throw new ArgumentNullException(nameof(eventProcessModel));

            _logger.Information("Adding {eventType} Event with {data}", eventProcessModel.EventType,
                _customSerializer.Serialize(eventProcessModel.Model));

            var eventData = new EventData(Guid.NewGuid(), eventProcessModel.EventType, false,
                _customDataConverter.ToBytes(eventProcessModel.Model), null);

            try
            {
                await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any,
                    eventData);
            }
            catch (Exception exception)
            {
                _logger.Error(exception,
                    $"Error saving {nameof(eventProcessModel)} with Correlation Id: {eventProcessModel.CorrelationId}");

                return new EventWriteResult {Error = exception.Message};
            }

            return new EventWriteResult();
        }
    }
}