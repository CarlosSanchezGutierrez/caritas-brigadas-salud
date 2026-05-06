using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Service : AuditableEntity
{
    private const int MaxCodeLength = 100;
    private const int MaxNameLength = 200;
    private const int MaxCategoryLength = 100;
    private const int MaxDescriptionLength = 500;

    private Service()
    {
        Code = string.Empty;
        Name = string.Empty;
        Category = string.Empty;
        Status = ServiceStatus.Active;
    }

    public Service(
        Guid id,
        Guid organizationId,
        string code,
        string name,
        string category,
        string? description = null,
        bool requiresConsent = true,
        bool requiresClinicalNotes = false,
        bool requiresFollowUpOption = true,
        bool requiresReferralOption = true,
        bool isSensitive = false)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        Code = NormalizeCode(code);
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        Category = NormalizeRequired(category, nameof(category), MaxCategoryLength);
        Description = NormalizeOptional(description, nameof(description), MaxDescriptionLength);
        RequiresConsent = requiresConsent;
        RequiresClinicalNotes = requiresClinicalNotes;
        RequiresFollowUpOption = requiresFollowUpOption;
        RequiresReferralOption = requiresReferralOption;
        IsSensitive = isSensitive;
        Status = ServiceStatus.Active;
    }

    public Guid OrganizationId { get; private set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string Category { get; private set; }

    public string? Description { get; private set; }

    public bool RequiresConsent { get; private set; }

    public bool RequiresClinicalNotes { get; private set; }

    public bool RequiresFollowUpOption { get; private set; }

    public bool RequiresReferralOption { get; private set; }

    public bool IsSensitive { get; private set; }

    public string Status { get; private set; }

    public bool IsActive => Status == ServiceStatus.Active;

    public void UpdateDetails(
        string name,
        string category,
        string? description)
    {
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        Category = NormalizeRequired(category, nameof(category), MaxCategoryLength);
        Description = NormalizeOptional(description, nameof(description), MaxDescriptionLength);
    }

    public void UpdateRules(
        bool requiresConsent,
        bool requiresClinicalNotes,
        bool requiresFollowUpOption,
        bool requiresReferralOption,
        bool isSensitive)
    {
        RequiresConsent = requiresConsent;
        RequiresClinicalNotes = requiresClinicalNotes;
        RequiresFollowUpOption = requiresFollowUpOption;
        RequiresReferralOption = requiresReferralOption;
        IsSensitive = isSensitive;
    }

    public void Activate()
    {
        Status = ServiceStatus.Active;
    }

    public void Deactivate()
    {
        Status = ServiceStatus.Inactive;
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static string NormalizeCode(string value)
    {
        var normalized = NormalizeRequired(value, nameof(Code), MaxCodeLength)
            .Trim()
            .ToUpperInvariant();

        if (normalized.Contains(' ', StringComparison.Ordinal))
        {
            throw new DomainException("Service code cannot contain spaces.");
        }

        return normalized;
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

public static class ServiceStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
}

public static class ServiceCode
{
    public const string GeneralMedicine = "GENERAL_MEDICINE";
    public const string Dentistry = "DENTISTRY";
    public const string Optometry = "OPTOMETRY";
    public const string Nutrition = "NUTRITION";
    public const string Psychology = "PSYCHOLOGY";
    public const string MedicationDelivery = "MEDICATION_DELIVERY";
    public const string MedicalReferral = "MEDICAL_REFERRAL";
    public const string Other = "OTHER";
}
