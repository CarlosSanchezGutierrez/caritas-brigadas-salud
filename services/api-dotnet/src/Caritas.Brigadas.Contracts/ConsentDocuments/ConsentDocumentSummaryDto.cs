namespace Caritas.Brigadas.Contracts.ConsentDocuments;

public sealed record ConsentDocumentSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid PatientId { get; init; }

    public Guid? VisitId { get; init; }

    public string ConsentType { get; init; } = string.Empty;

    public string DocumentVersion { get; init; } = string.Empty;

    public string? DocumentTextSnapshot { get; init; }

    public string? SignatureDataUrl { get; init; }

    public string? GuardianFullName { get; init; }

    public string? GuardianRelationship { get; init; }

    public Guid? SignedByUserId { get; init; }

    public DateTimeOffset? SignedAt { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public string SyncStatus { get; init; } = string.Empty;

    public bool IsDeleted { get; init; }
}
