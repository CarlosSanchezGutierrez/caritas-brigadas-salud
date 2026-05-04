namespace Caritas.Brigadas.Contracts.Api;

public sealed record ApiResponse<T>
{
    public bool Success { get; init; } = true;

    public T? Data { get; init; }

    public string? Message { get; init; }

    public string TraceId { get; init; } = string.Empty;

    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;

    public static ApiResponse<T> Ok(T data, string traceId, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            TraceId = traceId,
            TimestampUtc = DateTimeOffset.UtcNow
        };
    }
}
