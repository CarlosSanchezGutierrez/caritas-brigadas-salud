namespace Caritas.Brigadas.Contracts.Api;

public sealed record ApiErrorResponse
{
    public bool Success { get; init; } = false;

    public string ErrorCode { get; init; } = ApiErrorCodes.InternalServerError;

    public string Message { get; init; } = "An unexpected error occurred.";

    public IReadOnlyCollection<ApiErrorDetail> Details { get; init; } = Array.Empty<ApiErrorDetail>();

    public string TraceId { get; init; } = string.Empty;

    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;

    public static ApiErrorResponse Create(
        string errorCode,
        string message,
        string traceId,
        IReadOnlyCollection<ApiErrorDetail>? details = null)
    {
        return new ApiErrorResponse
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
            Details = details ?? Array.Empty<ApiErrorDetail>(),
            TraceId = traceId,
            TimestampUtc = DateTimeOffset.UtcNow
        };
    }
}
