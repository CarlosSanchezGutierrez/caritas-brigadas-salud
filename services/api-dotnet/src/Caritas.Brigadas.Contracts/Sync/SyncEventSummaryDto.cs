namespace Caritas.Brigadas.Contracts.Sync;

public sealed record SyncEventSummaryDto
{
    public Guid Id { get; init; }

    public Guid SyncBatchId { get; init; }

    public Guid OrganizationId { get; init; }

    public string LocalEventId { get; init; } = string.Empty;

    public string IdempotencyKey { get; init; } = string.Empty;

    public string EntityType { get; init; } = string.Empty;

    public Guid? EntityId { get; init; }

    public string Operation { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string? ErrorMessage { get; init; }

    public string? ConflictReason { get; init; }

    public DateTimeOffset? CreatedAtDevice { get; init; }

    public DateTimeOffset ReceivedAtServer { get; init; }

    public DateTimeOffset? ProcessedAt { get; init; }

    public bool IsPending { get; init; }

    public bool IsAccepted { get; init; }

    public bool IsRejected { get; init; }

    public bool IsConflict { get; init; }
}