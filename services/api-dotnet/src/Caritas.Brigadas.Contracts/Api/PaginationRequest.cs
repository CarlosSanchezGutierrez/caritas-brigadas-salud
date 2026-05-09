namespace Caritas.Brigadas.Contracts.Api;

public sealed record PaginationRequest
{
    public const int DefaultPageNumber = 1;

    public const int DefaultPageSize = 50;

    public const int MaxPageSize = 250;

    public const int MaxPageNumber = int.MaxValue / MaxPageSize;

    public int PageNumber { get; init; } = DefaultPageNumber;

    public int PageSize { get; init; } = DefaultPageSize;

    public int NormalizedPageNumber => PageNumber switch
    {
        < 1 => DefaultPageNumber,
        > MaxPageNumber => MaxPageNumber,
        _ => PageNumber
    };

    public int NormalizedPageSize => PageSize switch
    {
        < 1 => DefaultPageSize,
        > MaxPageSize => MaxPageSize,
        _ => PageSize
    };

    public int Skip
    {
        get
        {
            var offset = ((long)NormalizedPageNumber - 1L) * NormalizedPageSize;

            return offset > int.MaxValue
                ? int.MaxValue
                : (int)offset;
        }
    }
}