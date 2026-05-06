using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Integration;

public sealed class EndpointAuthorizationIntegrationTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EndpointAuthorizationIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("environment", "Development");

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Mode"] = "Development",
                    ["ConnectionStrings:SqlServer"] = string.Empty
                });
            });
        });
    }

    [Fact]
    public async Task Health_WhenNoAuthenticationHeaders_ReturnsOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var organizationId = Guid.NewGuid();

        var response = await client.GetAsync(
            $"/api/v1/organizations/{organizationId}/reports/summary", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WhenAuthenticatedWithoutPermission_ReturnsForbidden()
    {
        using var client = _factory.CreateClient();

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/organizations/{organizationId}/reports/summary");

        request.Headers.Add("X-Dev-User-Id", userId.ToString());
        request.Headers.Add("X-Dev-Organization-Id", organizationId.ToString());
        request.Headers.Add("X-Dev-Roles", "VIEWER");
        request.Headers.Add("X-Dev-Name", "Integration Test User");
        request.Headers.Add("X-Dev-Email", "integration.test@caritas.local");

        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task OrganizationScopedEndpoint_WhenAuthenticatedForDifferentOrganization_ReturnsForbidden()
    {
        using var client = _factory.CreateClient();

        var routeOrganizationId = Guid.NewGuid();
        var userOrganizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/organizations/{routeOrganizationId}/reports/summary");

        request.Headers.Add("X-Dev-User-Id", userId.ToString());
        request.Headers.Add("X-Dev-Organization-Id", userOrganizationId.ToString());
        request.Headers.Add("X-Dev-Roles", "SUPER_ADMIN_DISABLED_FOR_TEST");
        request.Headers.Add("X-Dev-Permissions", "reports.read");
        request.Headers.Add("X-Dev-Name", "Integration Test User");
        request.Headers.Add("X-Dev-Email", "integration.test@caritas.local");

        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WhenAuthenticatedAsSuperAdmin_ReachesController()
    {
        using var client = _factory.CreateClient();

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/organizations/{organizationId}/reports/summary");

        request.Headers.Add("X-Dev-User-Id", userId.ToString());
        request.Headers.Add("X-Dev-Organization-Id", organizationId.ToString());
        request.Headers.Add("X-Dev-Roles", "SUPER_ADMIN");
        request.Headers.Add("X-Dev-Name", "Integration Test User");
        request.Headers.Add("X-Dev-Email", "integration.test@caritas.local");

        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
