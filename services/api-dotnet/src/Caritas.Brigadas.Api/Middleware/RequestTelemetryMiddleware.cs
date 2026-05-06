using System.Diagnostics;

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

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            using var scope = _logger.BeginScope(new Dictionary<string, object?>
            {
                ["TraceId"] = context.TraceIdentifier,
                ["StatusCode"] = context.Response.StatusCode,
                ["ElapsedMilliseconds"] = stopwatch.ElapsedMilliseconds
            });
            _logger.LogInformation(
                "HTTP request responded {StatusCode} in {ElapsedMilliseconds} ms.",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
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
