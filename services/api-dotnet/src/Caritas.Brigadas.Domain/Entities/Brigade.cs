using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Brigade : AuditableEntity
{
    private const int MaxNameLength = 200;
    private const int MaxBrigadeTypeLength = 100;
    private const int MaxMunicipalityLength = 150;
    private const int MaxColonyLength = 150;
    private const int MaxLocationTextLength = 500;

    private Brigade()
    {
        Name = string.Empty;
        BrigadeType = string.Empty;
        Status = BrigadeState.Planned;
    }

    public Brigade(
        Guid id,
        Guid organizationId,
        string name,
        string brigadeType,
        DateOnly scheduledDate,
        Guid? communityId = null,
        string? municipality = null,
        string? colony = null,
        string? locationText = null,
        Guid? mobileUnitId = null,
        Guid? coordinatorUserId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        BrigadeType = NormalizeRequired(brigadeType, nameof(brigadeType), MaxBrigadeTypeLength);
        ScheduledDate = scheduledDate;
        CommunityId = communityId;
        Municipality = NormalizeOptional(municipality, nameof(municipality), MaxMunicipalityLength);
        Colony = NormalizeOptional(colony, nameof(colony), MaxColonyLength);
        LocationText = NormalizeOptional(locationText, nameof(locationText), MaxLocationTextLength);
        MobileUnitId = mobileUnitId;
        CoordinatorUserId = coordinatorUserId;
        Status = BrigadeState.Planned;
    }

    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; }

    public string BrigadeType { get; private set; }

    public DateOnly ScheduledDate { get; private set; }

    public DateTimeOffset? StartTime { get; private set; }

    public DateTimeOffset? EndTime { get; private set; }

    public Guid? CommunityId { get; private set; }

    public string? Municipality { get; private set; }

    public string? Colony { get; private set; }

    public string? LocationText { get; private set; }

    public Guid? MobileUnitId { get; private set; }

    public Guid? CoordinatorUserId { get; private set; }

    public string Status { get; private set; }

    public DateTimeOffset? OpenedAt { get; private set; }

    public Guid? OpenedByUserId { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public Guid? ClosedByUserId { get; private set; }

    public bool IsPlanned => Status == BrigadeState.Planned;

    public bool IsActive => Status == BrigadeState.Active;

    public bool IsClosed => Status == BrigadeState.Closed;

    public void UpdateSchedule(
        string name,
        string brigadeType,
        DateOnly scheduledDate,
        Guid? coordinatorUserId)
    {
        if (Status is BrigadeState.Closed or BrigadeState.Synced or BrigadeState.Reviewed)
        {
            throw new DomainException("Closed, synced or reviewed brigades cannot be rescheduled.");
        }

        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        BrigadeType = NormalizeRequired(brigadeType, nameof(brigadeType), MaxBrigadeTypeLength);
        ScheduledDate = scheduledDate;
        CoordinatorUserId = coordinatorUserId;
    }

    public void UpdateLocation(
        Guid? communityId,
        string? municipality,
        string? colony,
        string? locationText,
        Guid? mobileUnitId)
    {
        if (Status is BrigadeState.Closed or BrigadeState.Synced or BrigadeState.Reviewed)
        {
            throw new DomainException("Closed, synced or reviewed brigades cannot change location.");
        }

        CommunityId = communityId;
        Municipality = NormalizeOptional(municipality, nameof(municipality), MaxMunicipalityLength);
        Colony = NormalizeOptional(colony, nameof(colony), MaxColonyLength);
        LocationText = NormalizeOptional(locationText, nameof(locationText), MaxLocationTextLength);
        MobileUnitId = mobileUnitId;
    }

    public void Open(Guid openedByUserId, DateTimeOffset openedAt)
    {
        if (Status != BrigadeState.Planned)
        {
            throw new DomainException("Only planned brigades can be opened.");
        }

        Status = BrigadeState.Active;
        StartTime = openedAt;
        OpenedAt = openedAt;
        OpenedByUserId = RequireGuid(openedByUserId, nameof(openedByUserId));
    }

    public void Close(Guid closedByUserId, DateTimeOffset closedAt)
    {
        if (Status != BrigadeState.Active)
        {
            throw new DomainException("Only active brigades can be closed.");
        }

        Status = BrigadeState.Closed;
        EndTime = closedAt;
        ClosedAt = closedAt;
        ClosedByUserId = RequireGuid(closedByUserId, nameof(closedByUserId));
    }

    public void MarkSynced()
    {
        if (Status != BrigadeState.Closed)
        {
            throw new DomainException("Only closed brigades can be marked as synced.");
        }

        Status = BrigadeState.Synced;
    }

    public void MarkReviewed()
    {
        if (Status != BrigadeState.Synced)
        {
            throw new DomainException("Only synced brigades can be marked as reviewed.");
        }

        Status = BrigadeState.Reviewed;
    }

    public void Cancel()
    {
        if (Status is BrigadeState.Closed or BrigadeState.Synced or BrigadeState.Reviewed)
        {
            throw new DomainException("Closed, synced or reviewed brigades cannot be cancelled.");
        }

        Status = BrigadeState.Cancelled;
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

public static class BrigadeState
{
    public const string Planned = "planned";
    public const string Active = "active";
    public const string Closed = "closed";
    public const string Synced = "synced";
    public const string Reviewed = "reviewed";
    public const string Cancelled = "cancelled";
}
