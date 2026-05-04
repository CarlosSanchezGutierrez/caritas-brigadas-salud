namespace Caritas.Brigadas.Contracts.Security;

public sealed record RoleSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool IsSystemRole { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
