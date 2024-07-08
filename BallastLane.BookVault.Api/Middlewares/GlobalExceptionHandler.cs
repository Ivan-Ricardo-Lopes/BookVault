using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace BallastLane.BookVault.Api.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "{Message}", exception.Message);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var resultDetail = new
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request",
            };

            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(resultDetail), cancellationToken);

            return true;
        }
    }
}
