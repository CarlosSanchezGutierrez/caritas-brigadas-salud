using System.Diagnostics;
using System.Text;
using Caritas.Brigadas.Api.Extensions;

namespace Caritas.Brigadas.Api.Middleware;

public sealed class RequestTelemetryMiddleware
{
    private const int MaxLogValueLength = 256;
    private const int MaxEndpointRouteLength = 256;
    private const int MaxEndpointSegments = 8;

    private static readonly string[] SensitivePathSegments =
    [
        "patients",
        "patient-visits",
        "service-encounters",
        "form-responses",
        "consent-documents",
        "sync-batches"
    ];

    private static readonly IReadOnlyDictionary<string, string> AllowedHttpMethodsForLog =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["GET"] = "GET",
            ["POST"] = "POST",
            ["PUT"] = "PUT",
            ["PATCH"] = "PATCH",
            ["DELETE"] = "DELETE",
            ["HEAD"] = "HEAD",
            ["OPTIONS"] = "OPTIONS",
            ["TRACE"] = "TRACE",
            ["CONNECT"] = "CONNECT"
        };

    private static readonly IReadOnlyDictionary<string, string> AllowedPathSegmentsForLog =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["api"] = "api",
            ["v1"] = "v1",
            ["health"] = "health",
            ["live"] = "live",
            ["ready"] = "ready",
            ["organizations"] = "organizations",
            ["reports"] = "reports",
            ["summary"] = "summary",
            ["summary.csv"] = "summary.csv",
            ["audit-logs"] = "audit-logs",
            ["users"] = "users",
            ["roles"] = "roles",
            ["permissions"] = "permissions",
            ["brigades"] = "brigades",
            ["services"] = "services",
            ["catalogs"] = "catalogs",
            ["auth"] = "auth",
            ["me"] = "me"
        };

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
        var correlationId = SanitizeForLog(context.GetCorrelationId());

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

    private static string GetSafeHttpMethodForLog(string? method)
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            return "UNKNOWN";
        }

        var normalizedMethod = method.Trim().ToUpperInvariant();

        return AllowedHttpMethodsForLog.TryGetValue(normalizedMethod, out var trustedMethod)
            ? trustedMethod
            : "UNKNOWN";
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

        var rawSegments = rawPath.Split(
            '/',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (rawSegments.Length == 0)
        {
            return "/";
        }

        var route = new StringBuilder(capacity: Math.Min(rawPath.Length, MaxEndpointRouteLength));
        var emittedSegments = 0;

        foreach (var rawSegment in rawSegments)
        {
            if (emittedSegments >= MaxEndpointSegments)
            {
                route.Append("/[truncated]");
                break;
            }

            var safeSegment = GetSafePathSegmentForLog(rawSegment);

            if (route.Length + safeSegment.Length + 1 > MaxEndpointRouteLength)
            {
                route.Append("/[truncated]");
                break;
            }

            route.Append('/').Append(safeSegment);
            emittedSegments++;
        }

        return route.Length == 0
            ? "/"
            : route.ToString();
    }

    private static string GetSafePathSegmentForLog(string? segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
        {
            return "[segment]";
        }

        var normalizedSegment = SanitizeForLog(segment).ToLowerInvariant();

        if (Guid.TryParse(normalizedSegment, out _))
        {
            return "[id]";
        }

        if (IsNumericIdentifier(normalizedSegment))
        {
            return "[id]";
        }

        return AllowedPathSegmentsForLog.TryGetValue(normalizedSegment, out var trustedSegment)
            ? trustedSegment
            : "[segment]";
    }

    private static bool IsNumericIdentifier(string value)
    {
        if (value.Length == 0)
        {
            return false;
        }

        foreach (var character in value)
        {
            if (!char.IsDigit(character))
            {
                return false;
            }
        }

        return true;
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
                || character is '-' or '_' or '.' or '/' or ' ' or ':' or '[' or ']' or '(' or ')')
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
