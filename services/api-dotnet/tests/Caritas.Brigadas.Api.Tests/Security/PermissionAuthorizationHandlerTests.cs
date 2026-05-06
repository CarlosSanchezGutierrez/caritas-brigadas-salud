using System.Security.Claims;
using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenUserIsUnauthenticated_DoesNotSucceed()
    {
        var currentUser = CreateCurrentUserContext();
        var handler = new PermissionAuthorizationHandler(currentUser);
        var requirement = new PermissionRequirement(PermissionCodes.PatientsRead);
        var authorizationContext = CreateAuthorizationContext(requirement);

        await handler.HandleAsync(authorizationContext);

        Assert.False(authorizationContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WhenUserHasRequiredPermission_Succeeds()
    {
        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.PermissionCode, PermissionCodes.PatientsRead));

        var handler = new PermissionAuthorizationHandler(currentUser);
        var requirement = new PermissionRequirement(PermissionCodes.PatientsRead);
        var authorizationContext = CreateAuthorizationContext(requirement);

        await handler.HandleAsync(authorizationContext);

        Assert.True(authorizationContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WhenUserLacksRequiredPermission_DoesNotSucceed()
    {
        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.PermissionCode, PermissionCodes.PatientsRead));

        var handler = new PermissionAuthorizationHandler(currentUser);
        var requirement = new PermissionRequirement(PermissionCodes.ReportsExport);
        var authorizationContext = CreateAuthorizationContext(requirement);

        await handler.HandleAsync(authorizationContext);

        Assert.False(authorizationContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsSuperAdmin_SucceedsForAnyPermission()
    {
        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.RoleCode, RoleCodes.SuperAdmin));

        var handler = new PermissionAuthorizationHandler(currentUser);
        var requirement = new PermissionRequirement(PermissionCodes.AuditLogsRead);
        var authorizationContext = CreateAuthorizationContext(requirement);

        await handler.HandleAsync(authorizationContext);

        Assert.True(authorizationContext.HasSucceeded);
    }

    private static AuthorizationHandlerContext CreateAuthorizationContext(
        PermissionRequirement requirement)
    {
        return new AuthorizationHandlerContext(
            new[] { requirement },
            new ClaimsPrincipal(),
            resource: null);
    }

    private static HttpCurrentUserContext CreateCurrentUserContext(
        params Claim[] claims)
    {
        var principal = claims.Length == 0
            ? new ClaimsPrincipal(new ClaimsIdentity())
            : new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "Test"));

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
