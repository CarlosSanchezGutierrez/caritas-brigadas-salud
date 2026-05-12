using System.Security.Claims;
using Caritas.Brigadas.Api.Controllers;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class RoleAssignmentGuardrailTests
{
    [Fact]
    public async Task AssignRoleAsync_WhenTenantAdminAssignsSuperAdmin_ReturnsForbidBeforeRepository()
    {
        var controller = CreateControllerWithRole("ADMIN");

        var request = new AssignUserRoleRequest
        {
            RoleCode = RoleCodes.SuperAdmin
        };

        var result = await controller.AssignRoleAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            request,
            CancellationToken.None);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task AssignRoleAsync_WhenSuperAdminAssignsSuperAdmin_ContinuesToRepositoryPipeline()
    {
        var controller = CreateControllerWithRole(RoleCodes.SuperAdmin);

        var request = new AssignUserRoleRequest
        {
            RoleCode = RoleCodes.SuperAdmin
        };

        var result = await controller.AssignRoleAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            request,
            CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, objectResult.StatusCode);
    }

    private static RolesController CreateControllerWithRole(string roleCode)
    {
        var services = new ServiceCollection().BuildServiceProvider();

        return new RolesController(services)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[]
                        {
                            new Claim(CurrentUserClaimTypes.RoleCode, roleCode),
                            new Claim("permission", PermissionCodes.RolesAssign)
                        },
                        authenticationType: "TestAuth"))
                }
            }
        };
    }
}