using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class MediaRelease : AuditableEntity
{
    private const int MaxCampaignNameLength = 250;
    private const int MaxCommunityLength = 200;
    private const int MaxSignedByNameLength = 250;

    private MediaRelease()
    {
        Status = MediaReleaseStatus.Active;
    }

    public MediaRelease(
        Guid id,
        Guid organizationId,
        Guid patientId,
        Guid? visitId = null,
        string? campaignName = null,
        string? community = null,
        bool allowPhoto = false,
        bool allowVideo = false,
        string? signedByName = null,
        Guid? signatureId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        PatientId = RequireGuid(patientId, nameof(patientId));
        VisitId = visitId;
        CampaignName = NormalizeOptional(campaignName, nameof(campaignName), MaxCampaignNameLength);
        Community = NormalizeOptional(community, nameof(community), MaxCommunityLength);
        AllowPhoto = allowPhoto;
        AllowVideo = allowVideo;
        SignedByName = NormalizeOptional(signedByName, nameof(signedByName), MaxSignedByNameLength);
        SignatureId = signatureId;
        Status = MediaReleaseStatus.Active;
    }

    public Guid OrganizationId { get; private set; }

    public Guid PatientId { get; private set; }

    public Guid? VisitId { get; private set; }

    public string? CampaignName { get; private set; }

    public string? Community { get; private set; }

    public bool AllowPhoto { get; private set; }

    public bool AllowVideo { get; private set; }

    public string? SignedByName { get; private set; }

    public Guid? SignatureId { get; private set; }

    public string Status { get; private set; }

    public bool AllowsAnyMedia => AllowPhoto || AllowVideo;

    public void UpdatePermissions(bool allowPhoto, bool allowVideo)
    {
        AllowPhoto = allowPhoto;
        AllowVideo = allowVideo;
    }

    public void Revoke()
    {
        AllowPhoto = false;
        AllowVideo = false;
        Status = MediaReleaseStatus.Revoked;
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

public static class MediaReleaseStatus
{
    public const string Active = "active";
    public const string Revoked = "revoked";
}
