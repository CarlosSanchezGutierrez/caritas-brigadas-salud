using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal static class SyncPayloadReader
{
    public static bool TryReadObject<TRequest>(
        string payloadJson,
        string payloadLabel,
        JsonSerializerOptions serializerOptions,
        [NotNullWhen(true)] out TRequest? request,
        out string rejectionReason)
        where TRequest : class
    {
        try
        {
            using var document = JsonDocument.Parse(payloadJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                request = null;
                rejectionReason = $"{payloadLabel} payload must be a JSON object.";

                return false;
            }

            request = document.RootElement.Deserialize<TRequest>(serializerOptions);
        }
        catch (JsonException)
        {
            request = null;
            rejectionReason = $"{payloadLabel} payload JSON is invalid.";

            return false;
        }

        if (request is null)
        {
            rejectionReason = $"{payloadLabel} payload is required.";

            return false;
        }

        rejectionReason = string.Empty;

        return true;
    }
}