using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Permission : Entity
{
    private const int MaxCodeLength = 150;
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 500;
    private const int MaxModuleLength = 100;
    private const int MaxActionLength = 100;
    private const int MaxSensitivityLength = 50;

    private Permission()
    {
        Code = string.Empty;
        Name = string.Empty;
        Module = string.Empty;
        Action = string.Empty;
        SensitivityLevel = PermissionSensitivity.Normal;
    }

    public Permission(
        Guid id,
        string code,
        string name,
        string module,
        string action,
        string? description = null,
        string sensitivityLevel = PermissionSensitivity.Normal)
        : base(id)
    {
        Code = NormalizeCode(code);
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        Module = NormalizeRequired(module, nameof(module), MaxModuleLength);
        Action = NormalizeRequired(action, nameof(action), MaxActionLength);
        Description = NormalizeOptional(description, nameof(description), MaxDescriptionLength);
        SensitivityLevel = NormalizeRequired(sensitivityLevel, nameof(sensitivityLevel), MaxSensitivityLength);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string Module { get; private set; }

    public string Action { get; private set; }

    public string SensitivityLevel { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsSensitive =>
        SensitivityLevel == PermissionSensitivity.Sensitive ||
        SensitivityLevel == PermissionSensitivity.Restricted ||
        SensitivityLevel == PermissionSensitivity.Critical;

    private static string NormalizeCode(string value)
    {
        var normalized = NormalizeRequired(value, nameof(Code), MaxCodeLength)
            .Trim()
            .ToLowerInvariant();

        if (normalized.Contains(' ', StringComparison.Ordinal))
        {
            throw new DomainException("Permission code cannot contain spaces.");
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

public static class PermissionSensitivity
{
    public const string Normal = "normal";
    public const string Sensitive = "sensitive";
    public const string Restricted = "restricted";
    public const string Critical = "critical";
}
