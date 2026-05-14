using System.Security.Claims;
using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class HttpCurrentUserContextTests
{
    [Fact]
    public void IsAuthenticated_WhenNoAuthenticatedPrincipal_ReturnsFalse()
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        var currentUser = new HttpCurrentUserContext(accessor);

        Assert.False(currentUser.IsAuthenticated);
        Assert.Null(currentUser.UserId);
        Assert.Null(currentUser.OrganizationId);
        Assert.Empty(currentUser.Roles);
        Assert.Empty(currentUser.Permissions);
    }

    [Fact]
    public void CurrentUserContext_WhenClaimsExist_MapsUserOrganizationRolesAndPermissions()
    {
        var userId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, userId.ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, organizationId.ToString()),
            new Claim(CurrentUserClaimTypes.RoleCode, RoleCodes.HealthProvider),
            new Claim(CurrentUserClaimTypes.PermissionCode, PermissionCodes.PatientsRead));

        Assert.True(currentUser.IsAuthenticated);
        Assert.Equal(userId, currentUser.UserId);
        Assert.Equal(organizationId, currentUser.OrganizationId);
        Assert.True(currentUser.IsInRole(RoleCodes.HealthProvider));
        Assert.True(currentUser.HasPermission(PermissionCodes.PatientsRead));
        Assert.False(currentUser.HasPermission(PermissionCodes.ReportsExport));
    }

    [Fact]
    public void HasPermission_WhenUserIsSuperAdmin_AllowsAnyPermission()
    {
        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.RoleCode, RoleCodes.SuperAdmin));

        Assert.True(currentUser.HasPermission(PermissionCodes.AuditLogsRead));
        Assert.True(currentUser.HasPermission("custom.future.permission"));
    }

    [Fact]
    public void PermissionCodes_All_HasNoDuplicates()
    {
        var unique = PermissionCodes.All
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        Assert.Equal(PermissionCodes.All.Count, unique);
    }

    private static HttpCurrentUserContext CreateCurrentUserContext(
        params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        var principal = new ClaimsPrincipal(identity);

        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        return new HttpCurrentUserContext(accessor);
    }
}
