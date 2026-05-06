using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Brigades;

public sealed record AssignBrigadeServiceRequest
{
    [Required]
    [MaxLength(100)]
    public string ServiceCode { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int? CapacityEstimate { get; init; }

    public Guid? AssignedLeadUserId { get; init; }
}
