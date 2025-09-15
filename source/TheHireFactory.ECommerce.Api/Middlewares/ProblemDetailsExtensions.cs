using Microsoft.AspNetCore.Mvc;

namespace TheHireFactory.ECommerce.Api.Middlewares;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this Exception ex, int statusCode = StatusCodes.Status500InternalServerError)
    => new()
    {
        Title = "Beklenmeyen bir hata oluştu.",
        Detail = ex.Message,
        Status = statusCode,
        Type = "about:blank"
    };
}