using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class FormResponse : AuditableEntity
{
    private const int MaxHashLength = 128;

    private FormResponse()
    {
        ResponseJson = string.Empty;
        Status = FormResponseStatus.Draft;
        SyncStatus = SyncStatus.Synced;
    }

    public FormResponse(
        Guid id,
        Guid organizationId,
        Guid encounterId,
        Guid formTemplateId,
        string responseJson,
        bool createdOffline = false,
        Guid? deviceId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        EncounterId = RequireGuid(encounterId, nameof(encounterId));
        FormTemplateId = RequireGuid(formTemplateId, nameof(formTemplateId));
        ResponseJson = NormalizeJson(responseJson, nameof(responseJson));
        Status = FormResponseStatus.Draft;
        CreatedOffline = createdOffline;
        DeviceId = deviceId;
        SyncStatus = createdOffline ? SyncStatus.Pending : SyncStatus.Synced;
    }

    public Guid OrganizationId { get; private set; }

    public Guid EncounterId { get; private set; }

    public Guid FormTemplateId { get; private set; }

    public string ResponseJson { get; private set; }

    public string? ResponseHash { get; private set; }

    public Guid? CompletedByUserId { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public string Status { get; private set; }

    public bool CreatedOffline { get; private set; }

    public Guid? DeviceId { get; private set; }

    public DateTimeOffset? SubmittedAt { get; private set; }

    public DateTimeOffset? CapturedAt { get; private set; }

    public SyncStatus SyncStatus { get; private set; }

    public bool IsCompleted => Status == FormResponseStatus.Completed;

    public void UpdateResponse(string responseJson, string? responseHash = null)
    {
        if (Status == FormResponseStatus.Completed || Status == FormResponseStatus.Voided)
        {
            throw new DomainException("Completed or voided form responses cannot be updated.");
        }

        ResponseJson = NormalizeJson(responseJson, nameof(responseJson));
        ResponseHash = NormalizeOptional(responseHash, nameof(responseHash), MaxHashLength);
    }

    public void Complete(Guid completedByUserId, DateTimeOffset completedAt, string? responseHash = null)
    {
        if (Status == FormResponseStatus.Voided)
        {
            throw new DomainException("Voided form responses cannot be completed.");
        }

        Status = FormResponseStatus.Completed;
        CompletedByUserId = RequireGuid(completedByUserId, nameof(completedByUserId));
        CompletedAt = completedAt;
        ResponseHash = NormalizeOptional(responseHash, nameof(responseHash), MaxHashLength);
    }

    public void MarkNeedsReview()
    {
        if (Status == FormResponseStatus.Completed || Status == FormResponseStatus.Voided)
        {
            throw new DomainException("Completed or voided form responses cannot be marked for review.");
        }

        Status = FormResponseStatus.NeedsReview;
    }

    public void Void()
    {
        if (Status == FormResponseStatus.Completed)
        {
            throw new DomainException("Completed form responses cannot be voided without a correction workflow.");
        }

        Status = FormResponseStatus.Voided;
    }

    public void UpdateSyncStatus(SyncStatus syncStatus)
    {
        SyncStatus = syncStatus;
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static string NormalizeJson(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        return value.Trim();
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

public static class FormResponseStatus
{
    public const string Draft = "draft";
    public const string Completed = "completed";
    public const string NeedsReview = "needs_review";
    public const string Voided = "voided";
}
