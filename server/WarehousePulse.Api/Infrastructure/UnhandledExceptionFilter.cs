using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace WarehousePulse.Api.Infrastructure
{
    /// <summary>
    /// Last resort - log it and return a plain 500 instead of the yellow screen / stack trace.
    /// </summary>
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var request = context.Request;
            Trace.TraceError("{0} {1} failed: {2}", request.Method, request.RequestUri, context.Exception);

            context.Response = request.CreateResponse(HttpStatusCode.InternalServerError,
                new { message = "Something went wrong on the server, the error has been logged." });
        }
    }
}
