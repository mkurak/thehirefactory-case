using Microsoft.AspNetCore.Diagnostics;

namespace TheHireFactory.ECommerce.Api.Middlewares;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception");

        var problem = exception.ToProblemDetails(StatusCodes.Status500InternalServerError);
        httpContext.Response.StatusCode = problem.Status ?? 500;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true; // framework'e "ben hallettim" de
    }
}