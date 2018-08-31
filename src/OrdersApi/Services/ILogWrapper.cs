using Microsoft.AspNetCore.Mvc.Filters;

namespace OrdersApi.Services
{
    public interface ILogWrapper
    {
        void LogActionContext(ActionExecutingContext actionContext);

        void LogActionExecutedContext(ActionExecutedContext actionExecutedContext);
    }
}