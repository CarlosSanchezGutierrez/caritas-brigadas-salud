using Caritas.Brigadas.Api.Middleware;

namespace Caritas.Brigadas.Api.Extensions;

public static class HttpContextExtensions
{
    public static string GetCorrelationId(this HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value)
            && value is not null)
        {
            return value.ToString() ?? context.TraceIdentifier;
        }

        return context.TraceIdentifier;
    }
}
