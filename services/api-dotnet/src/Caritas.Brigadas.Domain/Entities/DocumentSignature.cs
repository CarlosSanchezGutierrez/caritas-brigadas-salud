using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class DocumentSignature : AuditableEntity
{
    private const int MaxSignedByNameLength = 250;
    private const int MaxSignatureFileUrlLength = 500;
    private const int MaxHashLength = 128;

    private DocumentSignature()
    {
        SignedByRole = DocumentSignatureRole.Patient;
        SignedAt = DateTimeOffset.UtcNow;
        SyncStatus = SyncStatus.Synced;
    }

    public DocumentSignature(
        Guid id,
        Guid organizationId,
        Guid documentTemplateId,
        DocumentSignatureRole signedByRole,
        DateTimeOffset signedAt,
        Guid? patientId = null,
        Guid? visitId = null,
        Guid? encounterId = null,
        Guid? guardianId = null,
        string? signedByName = null,
        string? signatureFileUrl = null,
        string? signatureHash = null,
        Guid? signedByUserId = null,
        bool createdOffline = false,
        Guid? deviceId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        DocumentTemplateId = RequireGuid(documentTemplateId, nameof(documentTemplateId));
        PatientId = patientId;
        VisitId = visitId;
        EncounterId = encounterId;
        GuardianId = guardianId;
        SignedByName = NormalizeOptional(signedByName, nameof(signedByName), MaxSignedByNameLength);
        SignedByRole = signedByRole;
        SignatureFileUrl = NormalizeOptional(signatureFileUrl, nameof(signatureFileUrl), MaxSignatureFileUrlLength);
        SignatureHash = NormalizeOptional(signatureHash, nameof(signatureHash), MaxHashLength);
        SignedAt = signedAt;
        SignedByUserId = signedByUserId;
        CreatedOffline = createdOffline;
        DeviceId = deviceId;
        SyncStatus = createdOffline ? SyncStatus.Pending : SyncStatus.Synced;
    }

    public Guid OrganizationId { get; private set; }

    public Guid DocumentTemplateId { get; private set; }

    public Guid? PatientId { get; private set; }

    public Guid? VisitId { get; private set; }

    public Guid? EncounterId { get; private set; }

    public Guid? GuardianId { get; private set; }

    public string? SignedByName { get; private set; }

    public DocumentSignatureRole SignedByRole { get; private set; }

    public string? SignatureFileUrl { get; private set; }

    public string? SignatureHash { get; private set; }

    public DateTimeOffset SignedAt { get; private set; }

    public Guid? SignedByUserId { get; private set; }

    public bool CreatedOffline { get; private set; }

    public Guid? DeviceId { get; private set; }

    public SyncStatus SyncStatus { get; private set; }

    public void UpdateSyncStatus(SyncStatus syncStatus)
    {
        SyncStatus = syncStatus;
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static string? NormalizeOptional(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }
}
