using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class ServiceEncounter : AuditableEntity
{
    private const int MaxEncounterFolioLength = 50;
    private const int MaxNotesSummaryLength = 1000;
    private const int MaxRecommendationsLength = 1000;

    private ServiceEncounter()
    {
        EncounterFolio = string.Empty;
        Status = EncounterStatus.Active;
        SyncStatus = SyncStatus.Synced;
    }

    public ServiceEncounter(
        Guid id,
        Guid organizationId,
        string encounterFolio,
        Guid visitId,
        Guid patientId,
        Guid brigadeId,
        Guid serviceId,
        Guid? providerUserId = null,
        DateTimeOffset? startedAt = null,
        bool createdOffline = false,
        Guid? deviceId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        EncounterFolio = NormalizeRequired(encounterFolio, nameof(encounterFolio), MaxEncounterFolioLength).ToUpperInvariant();
        VisitId = RequireGuid(visitId, nameof(visitId));
        PatientId = RequireGuid(patientId, nameof(patientId));
        BrigadeId = RequireGuid(brigadeId, nameof(brigadeId));
        ServiceId = RequireGuid(serviceId, nameof(serviceId));
        ProviderUserId = providerUserId;
        StartedAt = startedAt;
        Status = EncounterStatus.Active;
        CreatedOffline = createdOffline;
        DeviceId = deviceId;
        SyncStatus = createdOffline ? SyncStatus.Pending : SyncStatus.Synced;
    }

    public Guid OrganizationId { get; private set; }

    public string EncounterFolio { get; private set; }

    public Guid VisitId { get; private set; }

    public Guid PatientId { get; private set; }

    public Guid BrigadeId { get; private set; }

    public Guid ServiceId { get; private set; }

    public Guid? ProviderUserId { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? EndedAt { get; private set; }

    public EncounterStatus Status { get; private set; }

    public string? NotesSummary { get; private set; }

    public string? Recommendations { get; private set; }

    public bool RequiresFollowUp { get; private set; }

    public bool RequiresReferral { get; private set; }

    public bool CreatedOffline { get; private set; }

    public Guid? DeviceId { get; private set; }

    public SyncStatus SyncStatus { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public Guid? ClosedByUserId { get; private set; }

    public bool IsActive => Status == EncounterStatus.Active;

    public bool IsCompleted => Status == EncounterStatus.Completed;

    public bool NeedsReview => Status == EncounterStatus.NeedsReview;

    public void AssignProvider(Guid? providerUserId)
    {
        EnsureNotClosedOrCancelled("Provider cannot be changed after encounter is completed or cancelled.");

        ProviderUserId = providerUserId;
    }

    public void Start(DateTimeOffset startedAt)
    {
        EnsureNotClosedOrCancelled("Encounter cannot be started after it is completed or cancelled.");

        if (StartedAt.HasValue)
        {
            throw new DomainException("Encounter has already been started.");
        }

        StartedAt = startedAt;
    }

    public void UpdateClinicalSummary(
        string? notesSummary,
        string? recommendations)
    {
        EnsureNotClosedOrCancelled("Clinical summary cannot be changed after encounter is completed or cancelled.");

        NotesSummary = NormalizeOptional(notesSummary, nameof(notesSummary), MaxNotesSummaryLength);
        Recommendations = NormalizeOptional(recommendations, nameof(recommendations), MaxRecommendationsLength);
    }

    public void UpdateFollowUpAndReferral(
        bool requiresFollowUp,
        bool requiresReferral)
    {
        EnsureNotClosedOrCancelled("Follow-up and referral flags cannot be changed after encounter is completed or cancelled.");

        RequiresFollowUp = requiresFollowUp;
        RequiresReferral = requiresReferral;
    }

    public void Complete(Guid closedByUserId, DateTimeOffset endedAt)
    {
        if (Status != EncounterStatus.Active && Status != EncounterStatus.NeedsReview)
        {
            throw new DomainException("Only active or review-needed encounters can be completed.");
        }

        if (StartedAt.HasValue && endedAt < StartedAt.Value)
        {
            throw new DomainException("Encounter end time cannot be before start time.");
        }

        Status = EncounterStatus.Completed;
        EndedAt = endedAt;
        ClosedAt = endedAt;
        ClosedByUserId = RequireGuid(closedByUserId, nameof(closedByUserId));
    }

    public void Cancel()
    {
        EnsureNotClosedOrCancelled("Completed or cancelled encounters cannot be cancelled.");

        Status = EncounterStatus.Cancelled;
    }

    public void MarkNeedsReview()
    {
        if (Status == EncounterStatus.Completed)
        {
            throw new DomainException("Completed encounters cannot be marked for review.");
        }

        if (Status == EncounterStatus.Cancelled)
        {
            throw new DomainException("Cancelled encounters cannot be marked for review.");
        }

        Status = EncounterStatus.NeedsReview;
    }

    public void UpdateSyncStatus(SyncStatus syncStatus)
    {
        SyncStatus = syncStatus;
    }

    private void EnsureNotClosedOrCancelled(string message)
    {
        if (Status == EncounterStatus.Completed || Status == EncounterStatus.Cancelled)
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
