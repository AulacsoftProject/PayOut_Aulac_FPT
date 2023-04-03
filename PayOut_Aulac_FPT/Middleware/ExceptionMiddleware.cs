using System.Net;
using PayOut_Aulac_FPT.Core.Exceptions;
using PayOut_Aulac_FPT.DTO;

namespace Microsoft.eShopWeb.PublicApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = getStatusCode(exception);
        await context.Response.WriteAsJsonAsync(new ErrorResponse(
            exception.Message
        ));
    }

    private int getStatusCode(Exception exception)
    {
        var typeEx = exception.GetType();
        if (typeEx == typeof(NotificationException))
        {
            return (int)HttpStatusCode.OK;
        }
        return (int)HttpStatusCode.InternalServerError;
    }
}
