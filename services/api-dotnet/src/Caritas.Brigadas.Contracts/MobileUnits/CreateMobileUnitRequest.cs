using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.MobileUnits;

public sealed record CreateMobileUnitRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(100)]
    public string? UnitType { get; init; }

    [MaxLength(50)]
    public string? PlateNumber { get; init; }

    [MaxLength(500)]
    public string? Description { get; init; }
}
