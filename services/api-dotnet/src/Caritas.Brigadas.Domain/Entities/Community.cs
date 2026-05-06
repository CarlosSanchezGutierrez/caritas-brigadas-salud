using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Community : AuditableEntity
{
    private const int MaxStateLength = 100;
    private const int MaxMunicipalityLength = 150;
    private const int MaxColonyLength = 150;
    private const int MaxCommunityNameLength = 200;
    private const int MaxAddressReferenceLength = 500;
    private const int MaxRiskLevelLength = 50;

    private Community()
    {
        State = "Nuevo León";
        Municipality = string.Empty;
        Status = CommunityStatus.Active;
    }

    public Community(
        Guid id,
        Guid organizationId,
        string municipality,
        string? colony = null,
        string? communityName = null,
        string state = "Nuevo León")
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        State = NormalizeRequired(state, nameof(state), MaxStateLength);
        Municipality = NormalizeRequired(municipality, nameof(municipality), MaxMunicipalityLength);
        Colony = NormalizeOptional(colony, nameof(colony), MaxColonyLength);
        CommunityName = NormalizeOptional(communityName, nameof(communityName), MaxCommunityNameLength);
        Status = CommunityStatus.Active;
    }

    public Guid OrganizationId { get; private set; }

    public string State { get; private set; }

    public string Municipality { get; private set; }

    public string? Colony { get; private set; }

    public string? CommunityName { get; private set; }

    public string? AddressReference { get; private set; }

    public string? RiskLevel { get; private set; }

    public string Status { get; private set; }

    public bool IsActive => Status == CommunityStatus.Active;

    public void UpdateLocation(
        string state,
        string municipality,
        string? colony,
        string? communityName,
        string? addressReference)
    {
        State = NormalizeRequired(state, nameof(state), MaxStateLength);
        Municipality = NormalizeRequired(municipality, nameof(municipality), MaxMunicipalityLength);
        Colony = NormalizeOptional(colony, nameof(colony), MaxColonyLength);
        CommunityName = NormalizeOptional(communityName, nameof(communityName), MaxCommunityNameLength);
        AddressReference = NormalizeOptional(addressReference, nameof(addressReference), MaxAddressReferenceLength);
    }

    public void UpdateRiskLevel(string? riskLevel)
    {
        RiskLevel = NormalizeOptional(riskLevel, nameof(riskLevel), MaxRiskLevelLength);
    }

    public void Activate()
    {
        Status = CommunityStatus.Active;
    }

    public void Deactivate()
    {
        Status = CommunityStatus.Inactive;
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

public static class CommunityStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
}
