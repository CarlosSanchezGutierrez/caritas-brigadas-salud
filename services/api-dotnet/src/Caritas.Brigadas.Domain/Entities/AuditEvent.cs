using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class AuditEvent : Entity
{
    private const int MaxEntityTypeLength = 100;
    private const int MaxActionLength = 100;
    private const int MaxHashLength = 128;
    private const int MaxIpAddressLength = 100;
    private const int MaxUserAgentLength = 500;

    private AuditEvent()
    {
        EntityType = string.Empty;
        Action = string.Empty;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public AuditEvent(
        Guid id,
        Guid organizationId,
        string entityType,
        string action,
        DateTimeOffset createdAt,
        Guid? actorUserId = null,
        Guid? deviceId = null,
        Guid? entityId = null,
        string? oldValueHash = null,
        string? newValueHash = null,
        string? metadataJson = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? eventHash = null,
        string? previousEventHash = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        ActorUserId = actorUserId;
        DeviceId = deviceId;
        EntityType = NormalizeRequired(entityType, nameof(entityType), MaxEntityTypeLength).ToLowerInvariant();
        EntityId = entityId;
        Action = NormalizeRequired(action, nameof(action), MaxActionLength).ToLowerInvariant();
        OldValueHash = NormalizeOptional(oldValueHash, nameof(oldValueHash), MaxHashLength);
        NewValueHash = NormalizeOptional(newValueHash, nameof(newValueHash), MaxHashLength);
        MetadataJson = NormalizeOptionalJson(metadataJson);
        IpAddress = NormalizeOptional(ipAddress, nameof(ipAddress), MaxIpAddressLength);
        UserAgent = NormalizeOptional(userAgent, nameof(userAgent), MaxUserAgentLength);
        CreatedAt = createdAt;
        EventHash = NormalizeOptional(eventHash, nameof(eventHash), MaxHashLength);
        PreviousEventHash = NormalizeOptional(previousEventHash, nameof(previousEventHash), MaxHashLength);
    }

    public Guid OrganizationId { get; private set; }

    public Guid? ActorUserId { get; private set; }

    public Guid? DeviceId { get; private set; }

    public string EntityType { get; private set; }

    public Guid? EntityId { get; private set; }

    public string Action { get; private set; }

    public string? OldValueHash { get; private set; }

    public string? NewValueHash { get; private set; }

    public string? MetadataJson { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public string? EventHash { get; private set; }

    public string? PreviousEventHash { get; private set; }

    public bool HasIntegrityLink => !string.IsNullOrWhiteSpace(EventHash);

    public void AttachIntegrityHashes(string eventHash, string? previousEventHash)
    {
        EventHash = NormalizeRequired(eventHash, nameof(eventHash), MaxHashLength);
        PreviousEventHash = NormalizeOptional(previousEventHash, nameof(previousEventHash), MaxHashLength);
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

    private static string? NormalizeOptionalJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}

public static class AuditAction
{
    public const string UserLogin = "user.login";
    public const string UserLoginFailed = "user.login_failed";
    public const string PatientCreated = "patient.created";
    public const string PatientUpdated = "patient.updated";
    public const string VisitCreated = "visit.created";
    public const string EncounterCreated = "encounter.created";
    public const string FormCompleted = "form.completed";
    public const string DocumentSigned = "document.signed";
    public const string BrigadeOpened = "brigade.opened";
    public const string BrigadeClosed = "brigade.closed";
    public const string ExportCreated = "export.created";
    public const string DeviceApproved = "device.approved";
    public const string DeviceRevoked = "device.revoked";
    public const string PermissionChanged = "permission.changed";
    public const string SyncCompleted = "sync.completed";
    public const string DuplicateResolved = "duplicate.resolved";
    public const string AiRequested = "ai.requested";
}
