namespace Caritas.Brigadas.Contracts.Security;

public sealed record SecuritySeedResultDto
{
    public Guid OrganizationId { get; init; }

    public int RolesCreated { get; init; }

    public int PermissionsCreated { get; init; }

    public int RolePermissionsCreated { get; init; }

    public IReadOnlyCollection<string> RoleCodes { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> PermissionCodes { get; init; } = Array.Empty<string>();
}
