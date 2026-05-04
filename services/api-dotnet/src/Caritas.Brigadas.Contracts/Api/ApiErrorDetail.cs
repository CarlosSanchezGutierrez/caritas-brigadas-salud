namespace Caritas.Brigadas.Contracts.Api;

public sealed record ApiErrorDetail(
    string? Field,
    string Message,
    string? Code = null);
