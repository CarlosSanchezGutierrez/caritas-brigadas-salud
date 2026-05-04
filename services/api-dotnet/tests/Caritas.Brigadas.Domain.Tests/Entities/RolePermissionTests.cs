using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class RolePermissionTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateGrant()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var grantedAt = DateTimeOffset.UtcNow;
        var grantedByUserId = Guid.NewGuid();

        var grant = new RolePermission(
            Guid.NewGuid(),
            roleId,
            permissionId,
            grantedAt,
            grantedByUserId);

        Assert.Equal(roleId, grant.RoleId);
        Assert.Equal(permissionId, grant.PermissionId);
        Assert.Equal(grantedAt, grant.GrantedAt);
        Assert.Equal(grantedByUserId, grant.GrantedByUserId);
    }

    [Fact]
    public void Constructor_WithEmptyRoleId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new RolePermission(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                DateTimeOffset.UtcNow));
    }
}
