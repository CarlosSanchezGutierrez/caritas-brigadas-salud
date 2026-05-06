namespace Caritas.Brigadas.Contracts.Brigades;

public sealed record BrigadeServiceAssignmentDto
{
    public Guid Id { get; init; }

    public Guid BrigadeId { get; init; }

    public Guid ServiceId { get; init; }

    public string ServiceCode { get; init; } = string.Empty;

    public string ServiceName { get; init; } = string.Empty;

    public string ServiceCategory { get; init; } = string.Empty;

    public bool IsAvailable { get; init; }

    public int? CapacityEstimate { get; init; }

    public Guid? AssignedLeadUserId { get; init; }
}
