namespace Caritas.Brigadas.Domain.Entities;

public sealed class ConsentDocument
{
    private ConsentDocument()
    {
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid PatientId { get; private set; }

    public Guid? VisitId { get; private set; }

    public string ConsentType { get; private set; } = string.Empty;

    public string DocumentVersion { get; private set; } = string.Empty;

    public string? DocumentTextSnapshot { get; private set; }

    public string? SignatureDataUrl { get; private set; }

    public string? GuardianFullName { get; private set; }

    public string? GuardianRelationship { get; private set; }

    public Guid? SignedByUserId { get; private set; }

    public DateTimeOffset SignedAt { get; private set; }

    public bool CreatedOffline { get; private set; }

    public Guid? DeviceId { get; private set; }

    public string SyncStatus { get; private set; } = "Synced";

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
}
