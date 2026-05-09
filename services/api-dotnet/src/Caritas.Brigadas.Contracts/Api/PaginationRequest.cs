namespace Caritas.Brigadas.Contracts.Api;

public sealed record PaginationRequest
{
    public const int DefaultPageNumber = 1;

    public const int DefaultPageSize = 50;

    public const int MaxPageSize = 250;

    public int PageNumber { get; init; } = DefaultPageNumber;

    public int PageSize { get; init; } = DefaultPageSize;

    public int NormalizedPageNumber => PageNumber < 1
        ? DefaultPageNumber
        : PageNumber;

    public int NormalizedPageSize => PageSize switch
    {
        < 1 => DefaultPageSize,
        > MaxPageSize => MaxPageSize,
        _ => PageSize
    };

    public int Skip => (NormalizedPageNumber - 1) * NormalizedPageSize;
}