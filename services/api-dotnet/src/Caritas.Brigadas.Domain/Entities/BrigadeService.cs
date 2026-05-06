using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class BrigadeService : AuditableEntity
{
    private BrigadeService()
    {
    }

    public BrigadeService(
        Guid id,
        Guid brigadeId,
        Guid serviceId,
        int? capacityEstimate = null,
        Guid? assignedLeadUserId = null)
        : base(id)
    {
        BrigadeId = RequireGuid(brigadeId, nameof(brigadeId));
        ServiceId = RequireGuid(serviceId, nameof(serviceId));
        IsAvailable = true;
        SetCapacityEstimate(capacityEstimate);
        AssignedLeadUserId = assignedLeadUserId;
    }

    public Guid BrigadeId { get; private set; }

    public Guid ServiceId { get; private set; }

    public bool IsAvailable { get; private set; }

    public int? CapacityEstimate { get; private set; }

    public Guid? AssignedLeadUserId { get; private set; }

    public void MarkAvailable()
    {
        IsAvailable = true;
    }

    public void MarkUnavailable()
    {
        IsAvailable = false;
    }

    public void AssignLead(Guid? assignedLeadUserId)
    {
        AssignedLeadUserId = assignedLeadUserId;
    }

    public void SetCapacityEstimate(int? capacityEstimate)
    {
        if (capacityEstimate.HasValue && capacityEstimate.Value < 0)
        {
            throw new DomainException("Capacity estimate cannot be negative.");
        }

        CapacityEstimate = capacityEstimate;
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }
}
