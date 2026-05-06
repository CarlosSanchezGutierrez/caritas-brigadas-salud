using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class SyncEvent : Entity
{
    private const int MaxLocalEventIdLength = 150;
    private const int MaxEntityTypeLength = 100;
    private const int MaxOperationLength = 50;
    private const int MaxErrorMessageLength = 4000;
    private const int MaxConflictReasonLength = 4000;

    private SyncEvent()
    {
        LocalEventId = string.Empty;
        EntityType = string.Empty;
        Operation = SyncOperation.Create;
        PayloadJson = string.Empty;
        Status = SyncEventStatus.Pending;
        ReceivedAtServer = DateTimeOffset.UtcNow;
    }

    public SyncEvent(
        Guid id,
        Guid syncBatchId,
        Guid organizationId,
        string localEventId,
        string entityType,
        string operation,
        string payloadJson,
        Guid? entityId = null,
        DateTimeOffset? createdAtDevice = null,
        DateTimeOffset? receivedAtServer = null)
        : base(id)
    {
        SyncBatchId = RequireGuid(syncBatchId, nameof(syncBatchId));
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        LocalEventId = NormalizeRequired(localEventId, nameof(localEventId), MaxLocalEventIdLength);
        EntityType = NormalizeRequired(entityType, nameof(entityType), MaxEntityTypeLength).ToLowerInvariant();
        EntityId = entityId;
        Operation = NormalizeRequired(operation, nameof(operation), MaxOperationLength).ToLowerInvariant();
        PayloadJson = NormalizeJson(payloadJson, nameof(payloadJson));
        Status = SyncEventStatus.Pending;
        CreatedAtDevice = createdAtDevice;
        ReceivedAtServer = receivedAtServer ?? DateTimeOffset.UtcNow;
    }

    public Guid SyncBatchId { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string LocalEventId { get; private set; }

    public string EntityType { get; private set; }

    public Guid? EntityId { get; private set; }

    public string Operation { get; private set; }

    public string PayloadJson { get; private set; }

    public string Status { get; private set; }

    public string? ErrorMessage { get; private set; }

    public string? ConflictReason { get; private set; }

    public DateTimeOffset? CreatedAtDevice { get; private set; }

    public DateTimeOffset ReceivedAtServer { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public bool IsPending => Status == SyncEventStatus.Pending;

    public bool IsAccepted => Status == SyncEventStatus.Accepted;

    public bool IsRejected => Status == SyncEventStatus.Rejected;

    public bool IsConflict => Status == SyncEventStatus.Conflict;

    public void MarkProcessing()
    {
        if (Status != SyncEventStatus.Pending)
        {
            throw new DomainException("Only pending sync events can be marked as processing.");
        }

        Status = SyncEventStatus.Processing;
    }

    public void Accept(DateTimeOffset processedAt, Guid? serverEntityId = null)
    {
        if (Status == SyncEventStatus.Rejected || Status == SyncEventStatus.Conflict)
        {
            throw new DomainException("Rejected or conflict sync events cannot be accepted.");
        }

        Status = SyncEventStatus.Accepted;
        ProcessedAt = processedAt;

        if (serverEntityId.HasValue)
        {
            EntityId = serverEntityId;
        }

        ErrorMessage = null;
        ConflictReason = null;
    }

    public void Reject(DateTimeOffset processedAt, string errorMessage)
    {
        if (Status == SyncEventStatus.Accepted)
        {
            throw new DomainException("Accepted sync events cannot be rejected.");
        }

        Status = SyncEventStatus.Rejected;
        ProcessedAt = processedAt;
        ErrorMessage = NormalizeRequired(errorMessage, nameof(errorMessage), MaxErrorMessageLength);
        ConflictReason = null;
    }

    public void MarkConflict(DateTimeOffset processedAt, string conflictReason)
    {
        if (Status == SyncEventStatus.Accepted)
        {
            throw new DomainException("Accepted sync events cannot be marked as conflict.");
        }

        Status = SyncEventStatus.Conflict;
        ProcessedAt = processedAt;
        ConflictReason = NormalizeRequired(conflictReason, nameof(conflictReason), MaxConflictReasonLength);
        ErrorMessage = null;
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

    private static string NormalizeJson(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        return value.Trim();
    }
}

public static class SyncEventStatus
{
    public const string Pending = "pending";
    public const string Processing = "processing";
    public const string Accepted = "accepted";
    public const string Rejected = "rejected";
    public const string Conflict = "conflict";
}

public static class SyncOperation
{
    public const string Create = "create";
    public const string Update = "update";
    public const string Void = "void";
    public const string Sign = "sign";
    public const string Sync = "sync";
}
