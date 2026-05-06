using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class RolePermission : Entity
{
    private RolePermission()
    {
    }

    public RolePermission(
        Guid id,
        Guid roleId,
        Guid permissionId,
        DateTimeOffset grantedAt,
        Guid? grantedByUserId = null)
        : base(id)
    {
        RoleId = RequireGuid(roleId, nameof(roleId));
        PermissionId = RequireGuid(permissionId, nameof(permissionId));
        GrantedAt = grantedAt;
        GrantedByUserId = grantedByUserId;
    }

    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public DateTimeOffset GrantedAt { get; private set; }

    public Guid? GrantedByUserId { get; private set; }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }
}
