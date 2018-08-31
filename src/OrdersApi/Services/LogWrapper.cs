using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace OrdersApi.Services
{
    public class LogWrapper : ILogWrapper
    {
        private readonly ILogger _logger;
        private readonly ICustomSerializer _serializer;

        public LogWrapper(ILogger logger, ICustomSerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
        }

        public void LogActionContext(ActionExecutingContext actionContext)
        {
            _logger.Information(
                $"Executing Action: {actionContext.ActionDescriptor.DisplayName}, parameters: {string.Join(", ", actionContext.ActionArguments.Select(x => $"{x.Key}: {_serializer.SerializeSecurely(x.Value)}"))}");
        }

        public void LogActionExecutedContext(ActionExecutedContext actionExecutedContext)
        {
            _logger.Information($"Executed Action: {actionExecutedContext.ActionDescriptor.DisplayName}");
        }
    }
}