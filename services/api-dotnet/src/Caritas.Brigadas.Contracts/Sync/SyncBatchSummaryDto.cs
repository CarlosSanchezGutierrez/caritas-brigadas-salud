namespace Caritas.Brigadas.Contracts.Sync;

public sealed record SyncBatchSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid? UserId { get; init; }

    public Guid? BrigadeId { get; init; }

    public Guid? DeviceId { get; init; }

    public int EventsCount { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset? CompletedAt { get; init; }

    public string? ErrorSummary { get; init; }

    public bool IsCompleted { get; init; }
}
