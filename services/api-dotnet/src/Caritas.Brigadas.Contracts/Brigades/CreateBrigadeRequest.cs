using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Brigades;

public sealed record CreateBrigadeRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BrigadeType { get; init; } = string.Empty;

    public DateOnly ScheduledDate { get; init; }

    public Guid? CommunityId { get; init; }

    [MaxLength(150)]
    public string? Municipality { get; init; }

    [MaxLength(150)]
    public string? Colony { get; init; }

    [MaxLength(500)]
    public string? LocationText { get; init; }

    public Guid? MobileUnitId { get; init; }

    public Guid? CoordinatorUserId { get; init; }
}
