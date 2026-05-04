using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class User : AuditableEntity
{
    private const int MaxFullNameLength = 200;
    private const int MaxEmailLength = 200;
    private const int MaxPhoneLength = 50;
    private const int MaxUsernameLength = 100;

    private User()
    {
        FullName = string.Empty;
        Status = UserStatus.Active;
    }

    public User(
        Guid id,
        Guid organizationId,
        string fullName,
        string? email = null,
        string? phone = null,
        string? username = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        FullName = NormalizeRequired(fullName, nameof(fullName), MaxFullNameLength);
        Email = NormalizeOptional(email, nameof(email), MaxEmailLength)?.ToLowerInvariant();
        Phone = NormalizeOptional(phone, nameof(phone), MaxPhoneLength);
        Username = NormalizeOptional(username, nameof(username), MaxUsernameLength);
        Status = UserStatus.Active;
    }

    public Guid OrganizationId { get; private set; }

    public string FullName { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Username { get; private set; }

    public string Status { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    public DateTimeOffset? DeactivatedAt { get; private set; }

    public bool IsActive => Status == UserStatus.Active;

    public void UpdateProfile(
        string fullName,
        string? email,
        string? phone,
        string? username)
    {
        FullName = NormalizeRequired(fullName, nameof(fullName), MaxFullNameLength);
        Email = NormalizeOptional(email, nameof(email), MaxEmailLength)?.ToLowerInvariant();
        Phone = NormalizeOptional(phone, nameof(phone), MaxPhoneLength);
        Username = NormalizeOptional(username, nameof(username), MaxUsernameLength);
    }

    public void MarkLogin(DateTimeOffset loginAt)
    {
        LastLoginAt = loginAt;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        DeactivatedAt = null;
    }

    public void Deactivate(DateTimeOffset deactivatedAt)
    {
        Status = UserStatus.Inactive;
        DeactivatedAt = deactivatedAt;
    }

    public void Lock()
    {
        Status = UserStatus.Locked;
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

public static class UserStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
    public const string Locked = "locked";
}
