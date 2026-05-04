using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class MedicalReferral : AuditableEntity
{
    private const int MaxReferralFolioLength = 50;
    private const int MaxDestinationInstitutionLength = 250;
    private const int MaxReferralReasonLength = 1000;
    private const int MaxPriorityLength = 50;

    private MedicalReferral()
    {
        ReferralFolio = string.Empty;
        ReferralReason = string.Empty;
        Status = MedicalReferralStatus.Created;
    }

    public MedicalReferral(
        Guid id,
        Guid organizationId,
        Guid encounterId,
        Guid patientId,
        string referralFolio,
        string referralReason,
        string? destinationInstitution = null,
        string? priority = null,
        Guid? referredByUserId = null,
        Guid? providerSignatureId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        EncounterId = RequireGuid(encounterId, nameof(encounterId));
        PatientId = RequireGuid(patientId, nameof(patientId));
        ReferralFolio = NormalizeRequired(referralFolio, nameof(referralFolio), MaxReferralFolioLength).ToUpperInvariant();
        ReferralReason = NormalizeRequired(referralReason, nameof(referralReason), MaxReferralReasonLength);
        DestinationInstitution = NormalizeOptional(destinationInstitution, nameof(destinationInstitution), MaxDestinationInstitutionLength);
        Priority = NormalizeOptional(priority, nameof(priority), MaxPriorityLength)?.ToLowerInvariant();
        ReferredByUserId = referredByUserId;
        ProviderSignatureId = providerSignatureId;
        Status = MedicalReferralStatus.Created;
    }

    public Guid OrganizationId { get; private set; }

    public Guid EncounterId { get; private set; }

    public Guid PatientId { get; private set; }

    public string ReferralFolio { get; private set; }

    public string? DestinationInstitution { get; private set; }

    public string ReferralReason { get; private set; }

    public string? Priority { get; private set; }

    public Guid? ReferredByUserId { get; private set; }

    public Guid? ProviderSignatureId { get; private set; }

    public string Status { get; private set; }

    public bool IsCreated => Status == MedicalReferralStatus.Created;

    public bool IsCompleted => Status == MedicalReferralStatus.Completed;

    public void UpdateDetails(
        string referralReason,
        string? destinationInstitution,
        string? priority)
    {
        if (Status == MedicalReferralStatus.Cancelled || Status == MedicalReferralStatus.Completed)
        {
            throw new DomainException("Completed or cancelled referrals cannot be updated.");
        }

        ReferralReason = NormalizeRequired(referralReason, nameof(referralReason), MaxReferralReasonLength);
        DestinationInstitution = NormalizeOptional(destinationInstitution, nameof(destinationInstitution), MaxDestinationInstitutionLength);
        Priority = NormalizeOptional(priority, nameof(priority), MaxPriorityLength)?.ToLowerInvariant();
    }

    public void AttachProviderSignature(Guid providerSignatureId)
    {
        if (Status == MedicalReferralStatus.Cancelled)
        {
            throw new DomainException("Cancelled referrals cannot receive provider signatures.");
        }

        ProviderSignatureId = RequireGuid(providerSignatureId, nameof(providerSignatureId));
    }

    public void Complete()
    {
        if (Status == MedicalReferralStatus.Cancelled)
        {
            throw new DomainException("Cancelled referrals cannot be completed.");
        }

        Status = MedicalReferralStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == MedicalReferralStatus.Completed)
        {
            throw new DomainException("Completed referrals cannot be cancelled.");
        }

        Status = MedicalReferralStatus.Cancelled;
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

public static class MedicalReferralStatus
{
    public const string Created = "created";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";
}
