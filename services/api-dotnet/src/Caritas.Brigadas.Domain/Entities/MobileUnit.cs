using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class MobileUnit : AuditableEntity
{
    private const int MaxNameLength = 150;
    private const int MaxUnitTypeLength = 100;
    private const int MaxPlateNumberLength = 50;
    private const int MaxDescriptionLength = 500;

    private MobileUnit()
    {
        Name = string.Empty;
        Status = MobileUnitStatus.Active;
    }

    public MobileUnit(
        Guid id,
        Guid organizationId,
        string name,
        string? unitType = null,
        string? plateNumber = null,
        string? description = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        UnitType = NormalizeOptional(unitType, nameof(unitType), MaxUnitTypeLength);
        PlateNumber = NormalizeOptional(plateNumber, nameof(plateNumber), MaxPlateNumberLength);
        Description = NormalizeOptional(description, nameof(description), MaxDescriptionLength);
        Status = MobileUnitStatus.Active;
    }

    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; }

    public string? UnitType { get; private set; }

    public string? PlateNumber { get; private set; }

    public string? Description { get; private set; }

    public string Status { get; private set; }

    public bool IsActive => Status == MobileUnitStatus.Active;

    public void UpdateDetails(
        string name,
        string? unitType,
        string? plateNumber,
        string? description)
    {
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        UnitType = NormalizeOptional(unitType, nameof(unitType), MaxUnitTypeLength);
        PlateNumber = NormalizeOptional(plateNumber, nameof(plateNumber), MaxPlateNumberLength);
        Description = NormalizeOptional(description, nameof(description), MaxDescriptionLength);
    }

    public void Activate()
    {
        Status = MobileUnitStatus.Active;
    }

    public void Deactivate()
    {
        Status = MobileUnitStatus.Inactive;
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

public static class MobileUnitStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
}
