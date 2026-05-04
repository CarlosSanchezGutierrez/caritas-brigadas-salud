using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Contracts.Api;

namespace Caritas.Brigadas.Api.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                context.GetCorrelationId());

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiErrorResponse.Create(
                ApiErrorCodes.InternalServerError,
                "An unexpected server error occurred.",
                context.GetCorrelationId());

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
