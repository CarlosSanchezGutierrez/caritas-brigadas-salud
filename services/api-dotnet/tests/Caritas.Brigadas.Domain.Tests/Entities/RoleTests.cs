using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class RoleTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateRole()
    {
        var organizationId = Guid.NewGuid();

        var role = new Role(
            Guid.NewGuid(),
            organizationId,
            " super_admin ",
            " Superadministrador institucional ",
            " Control total ",
            isSystemRole: true);

        Assert.Equal(organizationId, role.OrganizationId);
        Assert.Equal("SUPER_ADMIN", role.Code);
        Assert.Equal("Superadministrador institucional", role.Name);
        Assert.Equal("Control total", role.Description);
        Assert.True(role.IsSystemRole);
        Assert.True(role.IsActive);
    }

    [Fact]
    public void Constructor_WithCodeContainingSpaces_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Role(Guid.NewGuid(), Guid.NewGuid(), "SUPER ADMIN", "Super Admin"));
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveStatus()
    {
        var role = new Role(Guid.NewGuid(), Guid.NewGuid(), "AUDITOR", "Auditor");

        role.Deactivate();

        Assert.Equal(RoleStatus.Inactive, role.Status);
        Assert.False(role.IsActive);
    }
}
