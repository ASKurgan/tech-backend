using EmailNotificationService.API.Common;

namespace EmailNotificationService.API.Middlewares;

/// <summary>
/// Exceptions handling middleware.
/// </summary>
public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandler(
        RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var response = new Response { Success = false, Message = ex.Message };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}