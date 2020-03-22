using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Production.Api.Exceptions.Handlers;
using Serilog;

namespace Production.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Log.Error(exception, exception.Message);

            var exceptionHandlers = context.RequestServices.GetServices<IExceptionHandler>();

            IActionResult result = null;

            foreach (var handler in exceptionHandlers)
            {
                result = handler.Handle(exception);
                if (result != null)
                {
                    break;
                }
            }

            result = result ?? new StatusCodeResult((int)HttpStatusCode.InternalServerError);

            await WriteResultAsync(context, result);
        }

        private async Task WriteResultAsync(HttpContext context, IActionResult result)
        {
            var routeData = context.GetRouteData() ?? new RouteData();
            var actionContext = new ActionContext(context, routeData, new ActionDescriptor());

            await result.ExecuteResultAsync(actionContext);
        }
    }
}
