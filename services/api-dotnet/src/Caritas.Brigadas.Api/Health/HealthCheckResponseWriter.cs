using System.Text.Json;
using Caritas.Brigadas.Api.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Caritas.Brigadas.Api.Health;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public static async Task WriteAsync(
        HttpContext context,
        HealthReport report)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(report);

        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString().ToLowerInvariant(),
            timestampUtc = DateTimeOffset.UtcNow,
            correlationId = context.GetCorrelationId(),
            totalDurationMilliseconds = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries
                .OrderBy(entry => entry.Key, StringComparer.Ordinal)
                .Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString().ToLowerInvariant(),
                    description = entry.Value.Description,
                    durationMilliseconds = entry.Value.Duration.TotalMilliseconds,
                    tags = entry.Value.Tags.OrderBy(tag => tag, StringComparer.Ordinal).ToArray()
                })
                .ToArray()
        };

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            payload,
            JsonOptions,
            context.RequestAborted);
    }
}
