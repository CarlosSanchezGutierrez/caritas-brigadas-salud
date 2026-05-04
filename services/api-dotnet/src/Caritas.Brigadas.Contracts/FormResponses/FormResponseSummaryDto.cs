namespace Caritas.Brigadas.Contracts.FormResponses;

public sealed record FormResponseSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid EncounterId { get; init; }

    public Guid FormTemplateId { get; init; }

    public string ResponseJson { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string SyncStatus { get; init; } = string.Empty;

    public Guid? SubmittedByUserId { get; init; }

    public DateTimeOffset? SubmittedAt { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public bool IsDeleted { get; init; }
}
