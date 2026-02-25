
using Microsoft.AspNetCore.Diagnostics;
using NLog;
using System.Net;
using System.Text.Json;

namespace InventoryManagementSystem.ExceptionHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Log the exception using NLog
            logger.Error(exception, "An unhandled exception occurred");

            httpContext.Response.ContentType = "application/json";

            int statusCode;
            string message;

            if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "You are not authorized to access this resource.";
            }
            else if (exception is ArgumentException || exception is ArgumentNullException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Invalid request. Please check your input and try again.";
            }
            else if (exception is NotImplementedException)
            {
                statusCode = (int)HttpStatusCode.NotImplemented;
                message = "This feature is not implemented yet.";
            }
            else if(exception is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.Conflict;
                message = "This data is already exist";
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred. Please try again later.";
            }

            httpContext.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
#if             DEBUG
                Detail = exception.Message
#endif
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);
            await httpContext.Response.WriteAsync(errorJson, cancellationToken);

            return true;
        }
    }
}

