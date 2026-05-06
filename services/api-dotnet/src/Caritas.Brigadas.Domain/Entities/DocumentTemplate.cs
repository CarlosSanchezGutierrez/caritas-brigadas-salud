using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class DocumentTemplate : AuditableEntity
{
    private const int MaxDocumentTypeLength = 100;
    private const int MaxTitleLength = 250;
    private const int MaxVersionLength = 50;
    private const int MaxFileUrlLength = 500;
    private const int MaxHashLength = 128;

    private DocumentTemplate()
    {
        DocumentType = string.Empty;
        Title = string.Empty;
        Version = string.Empty;
        IsActive = true;
    }

    public DocumentTemplate(
        Guid id,
        Guid organizationId,
        string documentType,
        string title,
        string version,
        string? contentText = null,
        string? fileUrl = null,
        Guid? appliesToServiceId = null,
        bool requiresPatientSignature = false,
        bool requiresGuardianSignature = false,
        bool requiresProviderSignature = false,
        string? documentHash = null,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? effectiveTo = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        DocumentType = NormalizeRequired(documentType, nameof(documentType), MaxDocumentTypeLength).ToLowerInvariant();
        Title = NormalizeRequired(title, nameof(title), MaxTitleLength);
        Version = NormalizeRequired(version, nameof(version), MaxVersionLength);
        ContentText = NormalizeOptional(contentText);
        FileUrl = NormalizeOptional(fileUrl, nameof(fileUrl), MaxFileUrlLength);
        AppliesToServiceId = appliesToServiceId;
        RequiresPatientSignature = requiresPatientSignature;
        RequiresGuardianSignature = requiresGuardianSignature;
        RequiresProviderSignature = requiresProviderSignature;
        DocumentHash = NormalizeOptional(documentHash, nameof(documentHash), MaxHashLength);
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        IsActive = true;

        ValidateEffectiveDates();
    }

    public Guid OrganizationId { get; private set; }

    public string DocumentType { get; private set; }

    public string Title { get; private set; }

    public string Version { get; private set; }

    public string? ContentText { get; private set; }

    public string? FileUrl { get; private set; }

    public Guid? AppliesToServiceId { get; private set; }

    public bool RequiresPatientSignature { get; private set; }

    public bool RequiresGuardianSignature { get; private set; }

    public bool RequiresProviderSignature { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset? EffectiveFrom { get; private set; }

    public DateTimeOffset? EffectiveTo { get; private set; }

    public string? DocumentHash { get; private set; }

    public Guid? ApprovedByUserId { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public bool RequiresAnySignature =>
        RequiresPatientSignature ||
        RequiresGuardianSignature ||
        RequiresProviderSignature;

    public void Approve(Guid approvedByUserId, DateTimeOffset approvedAt)
    {
        ApprovedByUserId = RequireGuid(approvedByUserId, nameof(approvedByUserId));
        ApprovedAt = approvedAt;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void ValidateEffectiveDates()
    {
        if (EffectiveFrom.HasValue &&
            EffectiveTo.HasValue &&
            EffectiveTo.Value <= EffectiveFrom.Value)
        {
            throw new DomainException("Effective end date must be after effective start date.");
        }
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
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

public static class DocumentType
{
    public const string PrivacyNotice = "privacy_notice";
    public const string GeneralInformedConsent = "general_informed_consent";
    public const string DentalInformedConsent = "dental_informed_consent";
    public const string OptometryInformedConsent = "optometry_informed_consent";
    public const string NutritionInformedConsent = "nutrition_informed_consent";
    public const string PsychologyInformedConsent = "psychology_informed_consent";
    public const string MedicationDeliveryConsent = "medication_delivery_consent";
    public const string MediaRelease = "media_release";
    public const string PatientReferral = "patient_referral";
    public const string GuardianAuthorization = "guardian_authorization";
    public const string Other = "other";
}
