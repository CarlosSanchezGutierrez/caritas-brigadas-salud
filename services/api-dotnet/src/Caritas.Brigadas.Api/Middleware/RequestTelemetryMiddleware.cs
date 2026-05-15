using System.Diagnostics;
using Caritas.Brigadas.Api.Extensions;

namespace Caritas.Brigadas.Api.Middleware;

public sealed class RequestTelemetryMiddleware
{
    private static readonly string[] SensitivePathSegments =
    [
        "patients",
        "patient-visits",
        "service-encounters",
        "form-responses",
        "consent-documents",
        "sync-batches"
    ];

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTelemetryMiddleware> _logger;

    public RequestTelemetryMiddleware(
        RequestDelegate next,
        ILogger<RequestTelemetryMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var stopwatch = Stopwatch.StartNew();
        var httpMethod = context.Request.Method;
        var sanitizedPath = SanitizePath(context.Request.Path);
        var correlationId = context.GetCorrelationId();

        var scopeProperties = new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId,
            ["RequestId"] = context.TraceIdentifier,
            ["HttpMethod"] = httpMethod,
            ["EndpointRoute"] = sanitizedPath,
            ["StatusCode"] = 0,
            ["ElapsedMilliseconds"] = 0
        };

        using var scope = _logger.BeginScope(scopeProperties);

        Exception? capturedException = null;

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            capturedException = exception;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;

            scopeProperties["StatusCode"] = statusCode;
            scopeProperties["ElapsedMilliseconds"] = elapsedMilliseconds;

            if (capturedException is not null)
            {
                _logger.LogError(
                    capturedException,
                    "HTTP request failed {HttpMethod} {EndpointRoute} with {StatusCode} in {ElapsedMilliseconds} ms.",
                    httpMethod,
                    sanitizedPath,
                    statusCode,
                    elapsedMilliseconds);
            }
            else if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    "HTTP request responded {StatusCode} for {HttpMethod} {EndpointRoute} in {ElapsedMilliseconds} ms.",
                    statusCode,
                    httpMethod,
                    sanitizedPath,
                    elapsedMilliseconds);
            }
            else if (statusCode >= StatusCodes.Status400BadRequest)
            {
                _logger.LogWarning(
                    "HTTP request responded {StatusCode} for {HttpMethod} {EndpointRoute} in {ElapsedMilliseconds} ms.",
                    statusCode,
                    httpMethod,
                    sanitizedPath,
                    elapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP request responded {StatusCode} for {HttpMethod} {EndpointRoute} in {ElapsedMilliseconds} ms.",
                    statusCode,
                    httpMethod,
                    sanitizedPath,
                    elapsedMilliseconds);
            }
        }
    }

    private static string SanitizePath(PathString path)
    {
        var rawPath = path.Value;

        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return "/";
        }

        foreach (var segment in SensitivePathSegments)
        {
            if (rawPath.Contains(segment, StringComparison.OrdinalIgnoreCase))
            {
                return "/api/v1/[sensitive-resource]";
            }
        }

        return rawPath;
    }
}
