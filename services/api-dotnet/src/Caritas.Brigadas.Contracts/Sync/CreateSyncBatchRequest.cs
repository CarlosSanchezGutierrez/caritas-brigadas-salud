using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Sync;

public sealed record CreateSyncBatchRequest
{
    public Guid UserId { get; init; }

    public Guid BrigadeId { get; init; }

    public Guid? DeviceId { get; init; }

    [Required]
    public string PayloadJson { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int? EventsCount { get; init; }

    public DateTimeOffset? StartedAt { get; init; }
}
