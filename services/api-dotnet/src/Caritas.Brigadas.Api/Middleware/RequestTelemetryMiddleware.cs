using System.Diagnostics;
using System.Text;
using Caritas.Brigadas.Api.Extensions;

namespace Caritas.Brigadas.Api.Middleware;

public sealed class RequestTelemetryMiddleware
{
    private const int MaxLogValueLength = 128;
    private const int MaxEndpointRouteLength = 128;
    private const int MaxEndpointSegments = 8;

    private static readonly string[] AllowedHttpMethodsForLog =
    [
        HttpMethods.Get,
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete,
        HttpMethods.Head,
        HttpMethods.Options,
        HttpMethods.Trace,
        HttpMethods.Connect
    ];

    private static readonly PathString[] SensitivePathSegments =
    [
        new("/api/v1/patients"),
        new("/api/v1/patient-visits"),
        new("/api/v1/service-encounters"),
        new("/api/v1/form-responses"),
        new("/api/v1/consent-documents"),
        new("/api/v1/sync-batches")
    ];

    private static readonly (PathString Prefix, string Route)[] AllowedPathSegmentsForLog =
    [
        (new PathString("/api/v1/health"), "/api/v1/health"),
        (new PathString("/api/v1/health/live"), "/api/v1/health/live"),
        (new PathString("/api/v1/health/ready"), "/api/v1/health/ready"),
        (new PathString("/api/v1/organizations"), "/api/v1/organizations/[id]"),
        (new PathString("/api/v1/reports"), "/api/v1/reports/[segment]"),
        (new PathString("/api/v1/audit-logs"), "/api/v1/audit-logs"),
        (new PathString("/api/v1/users"), "/api/v1/users/[id]"),
        (new PathString("/api/v1/roles"), "/api/v1/roles"),
        (new PathString("/api/v1/permissions"), "/api/v1/permissions"),
        (new PathString("/api/v1/brigades"), "/api/v1/brigades/[id]"),
        (new PathString("/api/v1/services"), "/api/v1/services"),
        (new PathString("/api/v1/catalogs"), "/api/v1/catalogs"),
        (new PathString("/api/v1/auth"), "/api/v1/auth/[segment]"),
        (new PathString("/api/v1/me"), "/api/v1/me")
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
        var httpMethod = GetSafeHttpMethodForLog(context.Request.Method);
        var sanitizedPath = SanitizePath(context.Request.Path);
        _ = context.GetCorrelationId();
        var correlationId = SanitizeForLog(context.TraceIdentifier);

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

    private static int GetMaxEndpointSegmentsForLog()
    {
        return MaxEndpointSegments;
    }

    private static string GetSafeHttpMethodForLog(string? method)
    {
        if (HttpMethods.IsGet(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Get);
        }

        if (HttpMethods.IsPost(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Post);
        }

        if (HttpMethods.IsPut(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Put);
        }

        if (HttpMethods.IsPatch(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Patch);
        }

        if (HttpMethods.IsDelete(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Delete);
        }

        if (HttpMethods.IsHead(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Head);
        }

        if (HttpMethods.IsOptions(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Options);
        }

        if (HttpMethods.IsTrace(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Trace);
        }

        if (HttpMethods.IsConnect(method))
        {
            return GetAllowedHttpMethod(HttpMethods.Connect);
        }

        return "UNKNOWN";
    }

    private static string GetAllowedHttpMethod(string method)
    {
        return Array.IndexOf(AllowedHttpMethodsForLog, method) >= 0
            ? method
            : "UNKNOWN";
    }

    private static string SanitizePath(PathString path)
    {
        if (!path.HasValue)
        {
            return "/";
        }

        if (path.Value?.Length > MaxEndpointRouteLength)
        {
            return "/api/v1/[truncated]";
        }

        if (MaxEndpointSegments <= 0)
        {
            return "/api/v1/[truncated]";
        }

        foreach (var sensitivePath in SensitivePathSegments)
        {
            if (path.StartsWithSegments(sensitivePath))
            {
                return "/api/v1/[sensitive-resource]";
            }
        }

        foreach (var allowedRoute in AllowedPathSegmentsForLog)
        {
            if (path.StartsWithSegments(allowedRoute.Prefix))
            {
                return allowedRoute.Route;
            }
        }

        return "/api/v1/[segment]";
    }

    private static string SanitizeForLog(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "UNKNOWN";
        }

        var builder = new StringBuilder(capacity: Math.Min(value.Length, MaxLogValueLength));

        foreach (var character in value.Trim())
        {
            if (builder.Length >= MaxLogValueLength)
            {
                break;
            }

            if (char.IsLetterOrDigit(character)
                || character is '-' or '_' or '.' or ':' or '[' or ']')
            {
                builder.Append(character);
            }
            else if (!char.IsControl(character))
            {
                builder.Append('_');
            }
        }

        var sanitized = builder.ToString().Trim();

        return string.IsNullOrWhiteSpace(sanitized)
            ? "UNKNOWN"
            : sanitized;
    }
}
