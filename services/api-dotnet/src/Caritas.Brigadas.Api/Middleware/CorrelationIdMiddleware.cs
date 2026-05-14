using Microsoft.Extensions.Primitives;

namespace Caritas.Brigadas.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private const int MaxCorrelationIdLength = 128;

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var correlationId = GetOrCreateCorrelationId(context);

        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        await _next(context);
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out StringValues value)
            && !StringValues.IsNullOrEmpty(value))
        {
            var candidate = value.ToString().Trim();

            if (IsValidCorrelationId(candidate))
            {
                return candidate;
            }
        }

        return context.TraceIdentifier;
    }

    private static bool IsValidCorrelationId(string value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length <= MaxCorrelationIdLength
            && value.All(IsAllowedCorrelationIdCharacter);
    }

    private static bool IsAllowedCorrelationIdCharacter(char value)
    {
        return char.IsAsciiLetterOrDigit(value)
            || value is '-' or '_' or '.' or ':';
    }
}
