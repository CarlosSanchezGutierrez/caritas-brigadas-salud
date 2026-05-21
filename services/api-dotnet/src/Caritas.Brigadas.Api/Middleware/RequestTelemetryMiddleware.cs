using System.Diagnostics;
using System.Text;
using Caritas.Brigadas.Api.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Caritas.Brigadas.Api.Middleware;

public sealed class RequestTelemetryMiddleware
{
    private const int MaxLogValueLength = 128;

    private static readonly string[] SensitiveEndpointTokens =
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
        var httpMethod = GetSafeHttpMethodForLog(context.Request.Method);
        var endpointRoute = GetSafeEndpointRouteForLog(context.GetEndpoint());
        var correlationId = SanitizeForLog(context.GetCorrelationId());

        var scopeProperties = new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId,
            ["RequestId"] = context.TraceIdentifier,
            ["HttpMethod"] = httpMethod,
            ["EndpointRoute"] = endpointRoute,
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
                    endpointRoute,
                    statusCode,
                    elapsedMilliseconds);
            }
            else if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    "HTTP request responded {StatusCode} for {HttpMethod} {EndpointRoute} in {ElapsedMilliseconds} ms.",
                    statusCode,
                    httpMethod,
                    endpointRoute,
                    elapsedMilliseconds);
            }
            else if (statusCode >= StatusCodes.Status400BadRequest)
            {
                _logger.LogWarning(
                    "HTTP request responded {StatusCode} for {HttpMethod} {EndpointRoute} in {ElapsedMilliseconds} ms.",
                    statusCode,
                    httpMethod,
                    endpointRoute,
                    elapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP request responded {StatusCode} for {HttpMethod} {EndpointRoute} in {ElapsedMilliseconds} ms.",
                    statusCode,
                    httpMethod,
                    endpointRoute,
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

        if (HttpMethods.IsGet(method))
        {
            return "GET";
        }

        if (HttpMethods.IsPost(method))
        {
            return "POST";
        }

        if (HttpMethods.IsPut(method))
        {
            return "PUT";
        }

        if (HttpMethods.IsPatch(method))
        {
            return "PATCH";
        }

        if (HttpMethods.IsDelete(method))
        {
            return "DELETE";
        }

        if (HttpMethods.IsHead(method))
        {
            return "HEAD";
        }

        if (HttpMethods.IsOptions(method))
        {
            return "OPTIONS";
        }

        if (HttpMethods.IsTrace(method))
        {
            return "TRACE";
        }

        if (HttpMethods.IsConnect(method))
        {
            return "CONNECT";
        }

        return "UNKNOWN";
    }

    private static string GetSafeEndpointRouteForLog(Endpoint? endpoint)
    {
        if (endpoint is null)
        {
            return "/[unmatched-endpoint]";
        }

        var controllerAction = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        var routeTemplate = controllerAction?.AttributeRouteInfo?.Template;

        if (string.IsNullOrWhiteSpace(routeTemplate))
        {
            return "/[mapped-endpoint]";
        }

        return ClassifyEndpointTemplateForLog(routeTemplate);
    }

    private static string ClassifyEndpointTemplateForLog(string routeTemplate)
    {
        foreach (var sensitiveToken in SensitiveEndpointTokens)
        {
            if (routeTemplate.Contains(sensitiveToken, StringComparison.OrdinalIgnoreCase))
            {
                return "/api/v1/[sensitive-resource]";
            }
        }

        if (routeTemplate.Contains("health", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/health";
        }

        if (routeTemplate.Contains("reports", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/organizations/[id]/reports/[segment]";
        }

        if (routeTemplate.Contains("audit-logs", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/organizations/[id]/audit-logs";
        }

        if (routeTemplate.Contains("organizations", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/organizations/[id]";
        }

        if (routeTemplate.Contains("users", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/users/[id]";
        }

        if (routeTemplate.Contains("roles", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/roles";
        }

        if (routeTemplate.Contains("permissions", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/permissions";
        }

        if (routeTemplate.Contains("auth", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/auth/[segment]";
        }

        if (routeTemplate.Contains("me", StringComparison.OrdinalIgnoreCase))
        {
            return "/api/v1/me";
        }

        return "/api/v1/[endpoint]";
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
