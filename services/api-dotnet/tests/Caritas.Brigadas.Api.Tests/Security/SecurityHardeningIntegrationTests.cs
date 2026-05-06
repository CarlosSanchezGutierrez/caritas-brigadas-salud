using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class SecurityHardeningIntegrationTests
{
    [Fact]
    public async Task RootEndpoint_IncludesSecurityHeaders()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(WebHostDefaults.EnvironmentKey, "Development");
                builder.UseSetting("Authentication:Mode", "Development");
                builder.UseSetting("Security:RateLimiting:Enabled", "false");
            });

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.True(response.Headers.Contains("X-Frame-Options"));
        Assert.True(response.Headers.Contains("Referrer-Policy"));
        Assert.True(response.Headers.Contains("Permissions-Policy"));
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        Assert.True(response.Headers.CacheControl?.NoStore);
    }

    [Fact]
    public async Task RateLimiter_ReturnsTooManyRequests_WhenLimitIsExceeded()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(WebHostDefaults.EnvironmentKey, "Development");
                builder.UseSetting("Authentication:Mode", "Development");
                builder.UseSetting("Security:RateLimiting:Enabled", "true");
                builder.UseSetting("Security:RateLimiting:PermitLimit", "1");
                builder.UseSetting("Security:RateLimiting:WindowMinutes", "1");
                builder.UseSetting("Security:RateLimiting:QueueLimit", "0");
            });

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var statusCodes = new List<HttpStatusCode>();

        for (var requestIndex = 0; requestIndex < 5; requestIndex++)
        {
            using var response = await client.GetAsync("/", TestContext.Current.CancellationToken);
            statusCodes.Add(response.StatusCode);
        }

        Assert.Contains(HttpStatusCode.TooManyRequests, statusCodes);
    }
}
