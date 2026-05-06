using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Security;

public sealed record AssignUserRoleRequest
{
    [Required]
    [MaxLength(100)]
    public string RoleCode { get; init; } = string.Empty;

    public DateTimeOffset? ExpiresAt { get; init; }
}
