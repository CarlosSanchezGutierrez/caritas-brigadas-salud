namespace Caritas.Brigadas.Contracts.Brigades;

public sealed record BrigadeSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string BrigadeType { get; init; } = string.Empty;

    public DateOnly ScheduledDate { get; init; }

    public DateTimeOffset? StartTime { get; init; }

    public DateTimeOffset? EndTime { get; init; }

    public Guid? CommunityId { get; init; }

    public string? Municipality { get; init; }

    public string? Colony { get; init; }

    public string? LocationText { get; init; }

    public Guid? MobileUnitId { get; init; }

    public Guid? CoordinatorUserId { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsPlanned { get; init; }

    public bool IsActive { get; init; }

    public bool IsClosed { get; init; }
}
