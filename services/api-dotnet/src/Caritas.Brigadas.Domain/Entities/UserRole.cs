using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class UserRole : Entity
{
    private UserRole()
    {
        Status = UserRoleStatus.Active;
    }

    public UserRole(
        Guid id,
        Guid userId,
        Guid roleId,
        Guid organizationId,
        DateTimeOffset assignedAt,
        Guid? assignedByUserId = null,
        DateTimeOffset? expiresAt = null)
        : base(id)
    {
        UserId = RequireGuid(userId, nameof(userId));
        RoleId = RequireGuid(roleId, nameof(roleId));
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        AssignedAt = assignedAt;
        AssignedByUserId = assignedByUserId;
        ExpiresAt = expiresAt;
        Status = UserRoleStatus.Active;

        if (expiresAt.HasValue && expiresAt.Value <= assignedAt)
        {
            throw new DomainException("Role assignment expiration must be after assignment date.");
        }
    }

    public Guid UserId { get; private set; }

    public Guid RoleId { get; private set; }

    public Guid OrganizationId { get; private set; }

    public DateTimeOffset AssignedAt { get; private set; }

    public Guid? AssignedByUserId { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }

    public string Status { get; private set; }

    public bool IsActiveAt(DateTimeOffset moment)
    {
        return Status == UserRoleStatus.Active &&
               (!ExpiresAt.HasValue || ExpiresAt.Value > moment);
    }

    public void Revoke()
    {
        Status = UserRoleStatus.Revoked;
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }
}

public static class UserRoleStatus
{
    public const string Active = "active";
    public const string Revoked = "revoked";
}
