using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Users;

public sealed record CreateUserRequest
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; init; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; init; }

    [MaxLength(50)]
    public string? Phone { get; init; }

    [MaxLength(100)]
    public string? Username { get; init; }
}
