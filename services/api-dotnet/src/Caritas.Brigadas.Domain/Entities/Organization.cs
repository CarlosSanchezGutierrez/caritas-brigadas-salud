using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Organization : AuditableEntity
{
    private const int MaxNameLength = 200;
    private const int MaxLegalNameLength = 250;
    private const int MaxRfcLength = 20;
    private const int MaxAddressLength = 500;
    private const int MaxPhoneLength = 50;
    private const int MaxEmailLength = 200;
    private const int MaxWebsiteLength = 200;
    private const int MaxLogoUrlLength = 500;
    private const int MaxColorLength = 20;
    private const int MaxFontFamilyLength = 100;

    private Organization()
    {
        Name = string.Empty;
        Status = OrganizationStatus.Active;
    }

    public Organization(
        Guid id,
        string name,
        string? legalName = null,
        string? rfc = null)
        : base(id)
    {
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        LegalName = NormalizeOptional(legalName, nameof(legalName), MaxLegalNameLength);
        Rfc = NormalizeOptional(rfc, nameof(rfc), MaxRfcLength);
        Status = OrganizationStatus.Active;
    }

    public string Name { get; private set; }

    public string? LegalName { get; private set; }

    public string? Rfc { get; private set; }

    public string? Address { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Website { get; private set; }

    public string? LogoUrl { get; private set; }

    public string? PrimaryColor { get; private set; }

    public string? SecondaryColor { get; private set; }

    public string? AccentColor { get; private set; }

    public string? FontFamily { get; private set; }

    public string Status { get; private set; }

    public bool IsActive => Status == OrganizationStatus.Active;

    public void UpdateIdentity(
        string name,
        string? legalName,
        string? rfc)
    {
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        LegalName = NormalizeOptional(legalName, nameof(legalName), MaxLegalNameLength);
        Rfc = NormalizeOptional(rfc, nameof(rfc), MaxRfcLength);
    }

    public void UpdateContact(
        string? address,
        string? phone,
        string? email,
        string? website)
    {
        Address = NormalizeOptional(address, nameof(address), MaxAddressLength);
        Phone = NormalizeOptional(phone, nameof(phone), MaxPhoneLength);
        Email = NormalizeOptional(email, nameof(email), MaxEmailLength);
        Website = NormalizeOptional(website, nameof(website), MaxWebsiteLength);
    }

    public void UpdateBranding(
        string? logoUrl,
        string? primaryColor,
        string? secondaryColor,
        string? accentColor,
        string? fontFamily)
    {
        LogoUrl = NormalizeOptional(logoUrl, nameof(logoUrl), MaxLogoUrlLength);
        PrimaryColor = NormalizeOptional(primaryColor, nameof(primaryColor), MaxColorLength);
        SecondaryColor = NormalizeOptional(secondaryColor, nameof(secondaryColor), MaxColorLength);
        AccentColor = NormalizeOptional(accentColor, nameof(accentColor), MaxColorLength);
        FontFamily = NormalizeOptional(fontFamily, nameof(fontFamily), MaxFontFamilyLength);
    }

    public void Activate()
    {
        Status = OrganizationStatus.Active;
    }

    public void Deactivate()
    {
        Status = OrganizationStatus.Inactive;
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

public static class OrganizationStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
}
