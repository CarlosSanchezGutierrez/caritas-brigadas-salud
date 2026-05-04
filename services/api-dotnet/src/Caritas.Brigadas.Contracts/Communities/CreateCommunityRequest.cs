using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Communities;

public sealed record CreateCommunityRequest
{
    [MaxLength(100)]
    public string State { get; init; } = "Nuevo León";

    [Required]
    [MaxLength(150)]
    public string Municipality { get; init; } = string.Empty;

    [MaxLength(150)]
    public string? Colony { get; init; }

    [MaxLength(200)]
    public string? CommunityName { get; init; }

    [MaxLength(500)]
    public string? AddressReference { get; init; }

    [MaxLength(50)]
    public string? RiskLevel { get; init; }
}
