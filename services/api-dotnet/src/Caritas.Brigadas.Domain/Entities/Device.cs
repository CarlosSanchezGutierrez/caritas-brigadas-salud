using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Device : AuditableEntity
{
    private const int MaxDeviceNameLength = 150;
    private const int MaxDeviceTypeLength = 50;
    private const int MaxPlatformLength = 50;
    private const int MaxOsVersionLength = 100;
    private const int MaxAppVersionLength = 100;
    private const int MaxOwnerTypeLength = 50;

    private Device()
    {
        DeviceType = string.Empty;
        Platform = string.Empty;
        OwnerType = DeviceOwnerType.Institutional;
    }

    public Device(
        Guid id,
        Guid organizationId,
        string deviceType,
        string platform,
        string ownerType,
        string? deviceName = null,
        Guid? assignedToUserId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        DeviceType = NormalizeRequired(deviceType, nameof(deviceType), MaxDeviceTypeLength).ToLowerInvariant();
        Platform = NormalizeRequired(platform, nameof(platform), MaxPlatformLength).ToLowerInvariant();
        OwnerType = NormalizeRequired(ownerType, nameof(ownerType), MaxOwnerTypeLength).ToLowerInvariant();
        DeviceName = NormalizeOptional(deviceName, nameof(deviceName), MaxDeviceNameLength);
        AssignedToUserId = assignedToUserId;
        RegisteredAt = DateTimeOffset.UtcNow;
    }

    public Guid OrganizationId { get; private set; }

    public string? DeviceName { get; private set; }

    public string DeviceType { get; private set; }

    public string Platform { get; private set; }

    public string? OsVersion { get; private set; }

    public string? AppVersion { get; private set; }

    public string OwnerType { get; private set; }

    public Guid? AssignedToUserId { get; private set; }

    public bool IsApproved { get; private set; }

    public bool IsRevoked { get; private set; }

    public DateTimeOffset? LastSyncAt { get; private set; }

    public DateTimeOffset RegisteredAt { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public Guid? ApprovedByUserId { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public Guid? RevokedByUserId { get; private set; }

    public bool CanSync => IsApproved && !IsRevoked;

    public void UpdateMetadata(
        string? deviceName,
        string? osVersion,
        string? appVersion,
        Guid? assignedToUserId)
    {
        DeviceName = NormalizeOptional(deviceName, nameof(deviceName), MaxDeviceNameLength);
        OsVersion = NormalizeOptional(osVersion, nameof(osVersion), MaxOsVersionLength);
        AppVersion = NormalizeOptional(appVersion, nameof(appVersion), MaxAppVersionLength);
        AssignedToUserId = assignedToUserId;
    }

    public void Approve(Guid approvedByUserId, DateTimeOffset approvedAt)
    {
        if (IsRevoked)
        {
            throw new DomainException("A revoked device cannot be approved.");
        }

        IsApproved = true;
        ApprovedByUserId = RequireGuid(approvedByUserId, nameof(approvedByUserId));
        ApprovedAt = approvedAt;
    }

    public void Revoke(Guid revokedByUserId, DateTimeOffset revokedAt)
    {
        IsRevoked = true;
        RevokedByUserId = RequireGuid(revokedByUserId, nameof(revokedByUserId));
        RevokedAt = revokedAt;
    }

    public void MarkSynced(DateTimeOffset lastSyncAt)
    {
        if (!CanSync)
        {
            throw new DomainException("Device cannot sync unless it is approved and not revoked.");
        }

        LastSyncAt = lastSyncAt;
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

public static class DeviceOwnerType
{
    public const string Institutional = "institutional";
    public const string Personal = "personal";
}

public static class DeviceType
{
    public const string Phone = "phone";
    public const string Tablet = "tablet";
    public const string Laptop = "laptop";
    public const string Desktop = "desktop";
}

public static class DevicePlatform
{
    public const string Ios = "ios";
    public const string Android = "android";
    public const string Web = "web";
    public const string Windows = "windows";
    public const string Macos = "macos";
}
