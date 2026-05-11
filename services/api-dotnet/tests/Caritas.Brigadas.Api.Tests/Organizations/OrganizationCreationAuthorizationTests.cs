using System.Security.Claims;
using Caritas.Brigadas.Api.Controllers;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Organizations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Organizations;

public sealed class OrganizationCreationAuthorizationTests
{
    [Fact]
    public async Task CreateAsync_WhenUserIsTenantAdmin_ReturnsForbidBeforeCreatingOrganization()
    {
        var services = new ServiceCollection().BuildServiceProvider();

        var controller = new OrganizationsController(services)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[]
                        {
                            new Claim("role_code", "ADMIN"),
                            new Claim("permission", PermissionCodes.OrganizationsWrite)
                        },
                        authenticationType: "TestAuth"))
                }
            }
        };

        var request = new CreateOrganizationRequest
        {
            Name = "Unauthorized Organization"
        };

        var result = await controller.CreateAsync(request, CancellationToken.None);

        Assert.IsType<ForbidResult>(result);
    }
}