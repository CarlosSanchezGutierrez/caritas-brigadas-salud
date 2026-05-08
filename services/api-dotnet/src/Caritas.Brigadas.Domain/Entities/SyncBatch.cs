using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class SyncBatch : Entity
{
    private const int MaxErrorSummaryLength = 4000;

    private SyncBatch()
    {
        Status = SyncBatchStatus.Received;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public SyncBatch(
        Guid id,
        Guid organizationId,
        Guid? deviceId,
        Guid userId,
        DateTimeOffset startedAt,
        Guid? brigadeId = null,
        int eventsCount = 0)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        DeviceId = deviceId == Guid.Empty
            ? throw new DomainException("deviceId must not be empty when provided.")
            : deviceId;
        UserId = RequireGuid(userId, nameof(userId));
        BrigadeId = brigadeId;
        StartedAt = startedAt;
        Status = SyncBatchStatus.Received;
        SetCounts(eventsCount, 0, 0, 0);
    }

    public Guid OrganizationId { get; private set; }

    public Guid? DeviceId { get; private set; }

    public Guid UserId { get; private set; }

    public Guid? BrigadeId { get; private set; }

    public DateTimeOffset StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public string Status { get; private set; }

    public int EventsCount { get; private set; }

    public int AcceptedCount { get; private set; }

    public int RejectedCount { get; private set; }

    public int ConflictCount { get; private set; }

    public string? ErrorSummary { get; private set; }

    public bool IsCompleted =>
        Status == SyncBatchStatus.Completed ||
        Status == SyncBatchStatus.CompletedWithErrors;

    public void MarkProcessing()
    {
        if (Status != SyncBatchStatus.Received)
        {
            throw new DomainException("Only received sync batches can be marked as processing.");
        }

        Status = SyncBatchStatus.Processing;
    }

    public void Complete(
        DateTimeOffset completedAt,
        int acceptedCount,
        int rejectedCount,
        int conflictCount,
        string? errorSummary = null)
    {
        if (Status != SyncBatchStatus.Received && Status != SyncBatchStatus.Processing)
        {
            throw new DomainException("Only received or processing sync batches can be completed.");
        }

        SetCounts(EventsCount, acceptedCount, rejectedCount, conflictCount);

        CompletedAt = completedAt;
        ErrorSummary = NormalizeOptional(errorSummary, nameof(errorSummary), MaxErrorSummaryLength);

        Status = rejectedCount > 0 || conflictCount > 0
            ? SyncBatchStatus.CompletedWithErrors
            : SyncBatchStatus.Completed;
    }

    public void Fail(DateTimeOffset completedAt, string errorSummary)
    {
        if (Status == SyncBatchStatus.Completed || Status == SyncBatchStatus.CompletedWithErrors)
        {
            throw new DomainException("Completed sync batches cannot be failed.");
        }

        CompletedAt = completedAt;
        ErrorSummary = NormalizeRequired(errorSummary, nameof(errorSummary), MaxErrorSummaryLength);
        Status = SyncBatchStatus.Failed;
    }

    private void SetCounts(
        int eventsCount,
        int acceptedCount,
        int rejectedCount,
        int conflictCount)
    {
        if (eventsCount < 0 ||
            acceptedCount < 0 ||
            rejectedCount < 0 ||
            conflictCount < 0)
        {
            throw new DomainException("Sync counters cannot be negative.");
        }

        if (acceptedCount + rejectedCount + conflictCount > eventsCount)
        {
            throw new DomainException("Processed sync event counts cannot exceed total event count.");
        }

        EventsCount = eventsCount;
        AcceptedCount = acceptedCount;
        RejectedCount = rejectedCount;
        ConflictCount = conflictCount;
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

public static class SyncBatchStatus
{
    public const string Received = "received";
    public const string Processing = "processing";
    public const string Completed = "completed";
    public const string CompletedWithErrors = "completed_with_errors";
    public const string Failed = "failed";
}
