using GestionaT.Api.Shared;
using GestionaT.Application.Common.Errors;
using System.Text.Json;

namespace GestionaT.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var error = AppErrorFactory.Internal(ex.Message);

                var apiError = new ApiError
                {
                    Code = error.Metadata["ErrorCode"]?.ToString() ?? "INTERNAL_ERROR",
                    Message = error.Message,
                    Detail = ExceptionHelper.GetDetail(ex, context.RequestServices),
                    Target = null
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var json = JsonSerializer.Serialize(new List<ApiError> { apiError }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                await context.Response.WriteAsync(json);
            }
        }
    }
}
