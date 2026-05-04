using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class PatientVisit : AuditableEntity
{
    private const int MaxVisitFolioLength = 50;

    private PatientVisit()
    {
        VisitFolio = string.Empty;
        VisitStatus = VisitStatus.Active;
        SyncStatus = SyncStatus.Synced;
    }

    public PatientVisit(
        Guid id,
        Guid organizationId,
        string visitFolio,
        Guid patientId,
        Guid brigadeId,
        DateTimeOffset? arrivalTime = null,
        Guid? registeredByUserId = null,
        bool createdOffline = false,
        Guid? deviceId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        VisitFolio = NormalizeRequired(visitFolio, nameof(visitFolio), MaxVisitFolioLength).ToUpperInvariant();
        PatientId = RequireGuid(patientId, nameof(patientId));
        BrigadeId = RequireGuid(brigadeId, nameof(brigadeId));
        ArrivalTime = arrivalTime;
        RegisteredByUserId = registeredByUserId;
        VisitStatus = VisitStatus.Active;
        CreatedOffline = createdOffline;
        DeviceId = deviceId;
        SyncStatus = createdOffline ? SyncStatus.Pending : SyncStatus.Synced;
    }

    public Guid OrganizationId { get; private set; }

    public string VisitFolio { get; private set; }

    public Guid PatientId { get; private set; }

    public Guid BrigadeId { get; private set; }

    public DateTimeOffset? ArrivalTime { get; private set; }

    public Guid? RegisteredByUserId { get; private set; }

    public VisitStatus VisitStatus { get; private set; }

    public bool CreatedOffline { get; private set; }

    public Guid? DeviceId { get; private set; }

    public SyncStatus SyncStatus { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public Guid? ClosedByUserId { get; private set; }

    public bool IsActive => VisitStatus == VisitStatus.Active;

    public bool IsClosed => VisitStatus == VisitStatus.Closed;

    public bool NeedsReview => VisitStatus == VisitStatus.NeedsReview;

    public void UpdateArrivalTime(DateTimeOffset? arrivalTime)
    {
        EnsureNotClosedOrCancelled("Arrival time cannot be changed after visit is closed or cancelled.");

        ArrivalTime = arrivalTime;
    }

    public void Close(Guid closedByUserId, DateTimeOffset closedAt)
    {
        if (VisitStatus != VisitStatus.Active && VisitStatus != VisitStatus.NeedsReview)
        {
            throw new DomainException("Only active or review-needed visits can be closed.");
        }

        VisitStatus = VisitStatus.Closed;
        ClosedAt = closedAt;
        ClosedByUserId = RequireGuid(closedByUserId, nameof(closedByUserId));
    }

    public void Cancel()
    {
        EnsureNotClosedOrCancelled("Closed or cancelled visits cannot be cancelled.");

        VisitStatus = VisitStatus.Cancelled;
    }

    public void MarkNeedsReview()
    {
        if (VisitStatus == VisitStatus.Cancelled)
        {
            throw new DomainException("Cancelled visits cannot be marked for review.");
        }

        if (VisitStatus == VisitStatus.Closed)
        {
            throw new DomainException("Closed visits cannot be marked for review.");
        }

        VisitStatus = VisitStatus.NeedsReview;
    }

    public void UpdateSyncStatus(SyncStatus syncStatus)
    {
        SyncStatus = syncStatus;
    }

    private void EnsureNotClosedOrCancelled(string message)
    {
        if (VisitStatus == VisitStatus.Closed || VisitStatus == VisitStatus.Cancelled)
        {
            throw new DomainException(message);
        }
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
}
