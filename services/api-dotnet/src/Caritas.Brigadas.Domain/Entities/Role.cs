using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Role : AuditableEntity
{
    private const int MaxCodeLength = 100;
    private const int MaxNameLength = 150;
    private const int MaxDescriptionLength = 500;

    private Role()
    {
        Code = string.Empty;
        Name = string.Empty;
        Status = RoleStatus.Active;
    }

    public Role(
        Guid id,
        Guid organizationId,
        string code,
        string name,
        string? description = null,
        bool isSystemRole = false)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        Code = NormalizeCode(code);
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        Description = NormalizeOptional(description, nameof(description), MaxDescriptionLength);
        IsSystemRole = isSystemRole;
        Status = RoleStatus.Active;
    }

    public Guid OrganizationId { get; private set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public bool IsSystemRole { get; private set; }

    public string Status { get; private set; }

    public bool IsActive => Status == RoleStatus.Active;

    public void UpdateDetails(string name, string? description)
    {
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        Description = NormalizeOptional(description, nameof(description), MaxDescriptionLength);
    }

    public void Activate()
    {
        Status = RoleStatus.Active;
    }

    public void Deactivate()
    {
        Status = RoleStatus.Inactive;
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
            throw new DomainException("Role code cannot contain spaces.");
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

public static class RoleStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
}
