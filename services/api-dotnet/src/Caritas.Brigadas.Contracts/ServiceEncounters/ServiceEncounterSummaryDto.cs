namespace Caritas.Brigadas.Contracts.ServiceEncounters;

public sealed record ServiceEncounterSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string EncounterFolio { get; init; } = string.Empty;

    public Guid VisitId { get; init; }

    public Guid PatientId { get; init; }

    public Guid BrigadeId { get; init; }

    public Guid ServiceId { get; init; }

    public string ServiceCode { get; init; } = string.Empty;

    public string ServiceName { get; init; } = string.Empty;

    public Guid? ProviderUserId { get; init; }

    public DateTimeOffset? StartedAt { get; init; }

    public DateTimeOffset? CompletedAt { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public string SyncStatus { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public bool IsCompleted { get; init; }

    public bool NeedsReview { get; init; }
}
