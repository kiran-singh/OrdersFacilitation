using Microsoft.AspNetCore.Mvc.Filters;
using OrdersApi.Services;

namespace OrdersApi.Filters
{
    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogWrapper _logger;

        public LogActionFilterAttribute(ILogWrapper logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            _logger.LogActionExecutedContext(actionExecutedContext);

            base.OnActionExecuted(actionExecutedContext);
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            _logger.LogActionContext(actionContext);

            base.OnActionExecuting(actionContext);
        }
    }
}