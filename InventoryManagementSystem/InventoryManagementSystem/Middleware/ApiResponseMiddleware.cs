using System.Net;
using System.Text.Json;

namespace InventoryManagementSystem.Middlewares
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseMiddleware> _logger;

        public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public class ApiResponse<T>
        {
            public bool Status { get; set; } = true;
            public string Message { get; set; } = "Success";
            public T Data { get; set; }
            public string HttpStatus { get; set; }
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path;
            // Log the path to see exactly what Swagger is sending.
            _logger.LogInformation("MIDDLEWARE CHECK: Request Path is {Path}", requestPath);

            // Use a more robust check for an exact, case-insensitive match.
            if (requestPath.Equals("/api/ACSChat/eventgrid-webhook", StringComparison.OrdinalIgnoreCase) ||
        requestPath.StartsWithSegments("/chatHub", StringComparison.OrdinalIgnoreCase) || requestPath.StartsWithSegments("/api/ResumeParse/parse", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("MIDDLEWARE BYPASSED for Event Grid Webhook.");
                await _next(context);
                return;
            }
            
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                if (ShouldBypassWrapping(context, responseBody))
                {
                    _logger.LogDebug(
                        "Bypassing API response wrapping for {Path}. Status: {StatusCode}, Content-Type: {ContentType}",
                        requestPath,
                        context.Response.StatusCode,
                        context.Response.ContentType);

                    context.Response.Body = originalBodyStream;
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                object? parsedData = null;
                if (!string.IsNullOrWhiteSpace(bodyText) && IsJson(bodyText))
                {
                    parsedData = JsonSerializer.Deserialize<object>(bodyText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                var apiResponse = new ApiResponse<object>
                {
                    Status = context.Response.StatusCode < 400,
                    Message = GetDefaultMessage(context.Response.StatusCode),
                    Data = parsedData,
                    HttpStatus = ((HttpStatusCode)context.Response.StatusCode).ToString()
                };

                context.Response.ContentType = "application/json";
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null, // Use PascalCase as defined in the class
                    PropertyNameCaseInsensitive = true
                };
                var wrappedResponse = JsonSerializer.Serialize(apiResponse, jsonOptions);
                context.Response.Body = originalBodyStream;
                await context.Response.WriteAsync(wrappedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Body = originalBodyStream;
                var errorResponse = new ApiResponse<object>
                {
                    Status = false,
                    Message = "Internal Server Error",
                    Data = null,
                    HttpStatus = HttpStatusCode.InternalServerError.ToString()
                };

                context.Response.ContentType = "application/json";
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null, // Use PascalCase as defined in the class
                    PropertyNameCaseInsensitive = true
                };
                var errorJson = JsonSerializer.Serialize(errorResponse, jsonOptions);
                await context.Response.WriteAsync(errorJson);
            }
        }

        private static bool ShouldBypassWrapping(HttpContext context, MemoryStream responseBody)
        {
            if (responseBody.Length == 0)
            {
                return false;
            }

            if (context.Response.Headers.ContainsKey("Content-Disposition"))
            {
                return true;
            }

            var contentType = context.Response.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return !contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
                && !contentType.Contains("+json", StringComparison.OrdinalIgnoreCase)
                && !contentType.StartsWith("text/json", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsJson(string input)
        {
            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) ||
                   (input.StartsWith("[") && input.EndsWith("]"));
        }

        private string GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                >= 200 and < 300 => "Success",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                409=> "A conflict occurred: the resource already exists.",
                422=>"Status Not Active",
                _ => "Unhandled Response"
            };
        }
    }

}
