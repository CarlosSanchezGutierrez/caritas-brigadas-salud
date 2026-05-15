using Caritas.Brigadas.Application.Security;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class PermissionClassificationContractTests
{
    [Fact]
    public void PermissionCodes_AllPermissions_AreClassifiedAsGlobalOnlyOrTenantScoped()
    {
        var classified = PermissionCodes.GlobalOnly
            .Concat(PermissionCodes.TenantScoped)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.All(PermissionCodes.All, permission =>
            Assert.Contains(permission, classified));
    }

    [Fact]
    public void PermissionCodes_GlobalOnlyAndTenantScoped_DoNotOverlap()
    {
        var overlap = PermissionCodes.GlobalOnly
            .Intersect(PermissionCodes.TenantScoped, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.Empty(overlap);
    }

    [Fact]
    public void PermissionCodes_TenantScopedPermissions_AreIncludedInAll()
    {
        Assert.All(PermissionCodes.TenantScoped, permission =>
            Assert.Contains(permission, PermissionCodes.All));
    }

    [Fact]
    public void PermissionCodes_GlobalOnlyPermissions_AreIncludedInAll()
    {
        Assert.All(PermissionCodes.GlobalOnly, permission =>
            Assert.Contains(permission, PermissionCodes.All));
    }

    [Fact]
    public void PermissionCodes_OrganizationsWrite_IsGlobalOnly()
    {
        Assert.Contains(PermissionCodes.OrganizationsWrite, PermissionCodes.GlobalOnly);
        Assert.DoesNotContain(PermissionCodes.OrganizationsWrite, PermissionCodes.TenantScoped);
    }

    [Fact]
    public void PermissionCodes_OrganizationsRead_IsTenantScoped()
    {
        Assert.Contains(PermissionCodes.OrganizationsRead, PermissionCodes.TenantScoped);
        Assert.DoesNotContain(PermissionCodes.OrganizationsRead, PermissionCodes.GlobalOnly);
    }
}
