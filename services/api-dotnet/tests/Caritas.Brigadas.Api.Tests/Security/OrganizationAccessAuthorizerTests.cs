using System.Security.Claims;
using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class OrganizationAccessAuthorizerTests
{
    [Fact]
    public void CanAccessOrganization_WhenUserIsNotAuthenticated_ReturnsFalse()
    {
        var organizationId = Guid.NewGuid();
        var currentUser = CreateCurrentUserContext();
        var authorizer = new OrganizationAccessAuthorizer(currentUser);

        var result = authorizer.CanAccessOrganization(organizationId);

        Assert.False(result);
    }

    [Fact]
    public void CanAccessOrganization_WhenOrganizationIdIsEmpty_ReturnsFalse()
    {
        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, Guid.NewGuid().ToString()));

        var authorizer = new OrganizationAccessAuthorizer(currentUser);

        var result = authorizer.CanAccessOrganization(Guid.Empty);

        Assert.False(result);
    }

    [Fact]
    public void CanAccessOrganization_WhenUserBelongsToOrganization_ReturnsTrue()
    {
        var organizationId = Guid.NewGuid();

        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, organizationId.ToString()));

        var authorizer = new OrganizationAccessAuthorizer(currentUser);

        var result = authorizer.CanAccessOrganization(organizationId);

        Assert.True(result);
    }

    [Fact]
    public void CanAccessOrganization_WhenUserBelongsToDifferentOrganization_ReturnsFalse()
    {
        var requestedOrganizationId = Guid.NewGuid();
        var userOrganizationId = Guid.NewGuid();

        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, userOrganizationId.ToString()));

        var authorizer = new OrganizationAccessAuthorizer(currentUser);

        var result = authorizer.CanAccessOrganization(requestedOrganizationId);

        Assert.False(result);
    }

    [Fact]
    public void CanAccessOrganization_WhenUserIsSuperAdmin_ReturnsTrue()
    {
        var requestedOrganizationId = Guid.NewGuid();

        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.RoleCode, RoleCodes.SuperAdmin));

        var authorizer = new OrganizationAccessAuthorizer(currentUser);

        var result = authorizer.CanAccessOrganization(requestedOrganizationId);

        Assert.True(result);
    }

    [Fact]
    public void EnsureCanAccessOrganization_WhenUserCannotAccessOrganization_ThrowsUnauthorizedAccessException()
    {
        var requestedOrganizationId = Guid.NewGuid();
        var userOrganizationId = Guid.NewGuid();

        var currentUser = CreateCurrentUserContext(
            new Claim(CurrentUserClaimTypes.UserId, Guid.NewGuid().ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, userOrganizationId.ToString()));

        var authorizer = new OrganizationAccessAuthorizer(currentUser);

        Assert.Throws<UnauthorizedAccessException>(() =>
            authorizer.EnsureCanAccessOrganization(requestedOrganizationId));
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
