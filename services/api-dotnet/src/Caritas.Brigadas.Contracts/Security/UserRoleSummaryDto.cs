namespace Caritas.Brigadas.Contracts.Security;

public sealed record UserRoleSummaryDto
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public Guid RoleId { get; init; }

    public Guid OrganizationId { get; init; }

    public string RoleCode { get; init; } = string.Empty;

    public string RoleName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateTimeOffset AssignedAt { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }

    public bool IsActive { get; init; }
}
