using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Organizations;

public sealed record CreateOrganizationRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(250)]
    public string? LegalName { get; init; }

    [MaxLength(20)]
    public string? Rfc { get; init; }

    [MaxLength(500)]
    public string? Address { get; init; }

    [MaxLength(50)]
    public string? Phone { get; init; }

    [MaxLength(200)]
    public string? Email { get; init; }

    [MaxLength(200)]
    public string? Website { get; init; }
}
