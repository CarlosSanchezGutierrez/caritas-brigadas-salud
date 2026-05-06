namespace Caritas.Brigadas.Contracts.PatientVisits;

public sealed record PatientVisitSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string VisitFolio { get; init; } = string.Empty;

    public Guid PatientId { get; init; }

    public Guid BrigadeId { get; init; }

    public DateTimeOffset? ArrivalTime { get; init; }

    public Guid? RegisteredByUserId { get; init; }

    public string VisitStatus { get; init; } = string.Empty;

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public string SyncStatus { get; init; } = string.Empty;

    public DateTimeOffset? ClosedAt { get; init; }

    public Guid? ClosedByUserId { get; init; }

    public bool IsActive { get; init; }

    public bool IsClosed { get; init; }

    public bool NeedsReview { get; init; }
}
