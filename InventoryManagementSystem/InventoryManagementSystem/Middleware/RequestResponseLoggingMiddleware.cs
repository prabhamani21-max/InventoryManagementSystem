using System.Text;

namespace InventoryManagementSystem.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Enable request body buffering
            context.Request.EnableBuffering();

            // Read and log the request body if available
            string requestBody = string.Empty;
            if (context.Request.ContentLength > 0)
            {
                context.Request.Body.Position = 0;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
            }
            var logger = NLog.LogManager.GetCurrentClassLogger();

            logger.Info($"[Request] {context.Request.Method} {context.Request.Path} | Body: {requestBody}");

            // Capture the original response stream
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Read and conditionally log the response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600)
            {
                // Only log response body for error responses, and optionally strip large HTML bodies
                if (IsHtml(responseText))
                {
                    logger.Info($"[Response] {context.Response.StatusCode}: HTML content not logged.");
                }
                else
                {
                    logger.Info($"[Response] {context.Response.StatusCode}: {responseText}");
                }
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private bool IsHtml(string input)
        {
            return input.TrimStart().StartsWith("<!DOCTYPE html", System.StringComparison.OrdinalIgnoreCase)
                || input.TrimStart().StartsWith("<html", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
