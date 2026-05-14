using System.Net;
using System.Text.Json;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Integration;

public sealed class P3HealthEndpointIntegrationTests
{
    [Fact]
    public async Task LiveHealthEndpoint_ReturnsJsonWithoutAuthentication()
    {
        await using var factory = CreateFactory($"p3-health-live-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.Add("X-Correlation-Id", "p3-health-live-correlation");

        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.True(response.Headers.TryGetValues("X-Correlation-Id", out var correlationValues));
        Assert.Contains("p3-health-live-correlation", correlationValues);

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.Equal("healthy", root.GetProperty("status").GetString());
        Assert.Equal("p3-health-live-correlation", root.GetProperty("correlationId").GetString());
        Assert.True(root.TryGetProperty("timestampUtc", out _));
        Assert.True(root.TryGetProperty("totalDurationMilliseconds", out _));

        var checks = root.GetProperty("checks");
        Assert.Equal(1, checks.GetArrayLength());
        Assert.Equal("api-live", checks[0].GetProperty("name").GetString());
        Assert.Equal("healthy", checks[0].GetProperty("status").GetString());

        Assert.DoesNotContain("ConnectionStrings", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SqlServer", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Password", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PayloadJson", responseBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReadyHealthEndpoint_ReturnsDatabaseConnectivitySignalWithoutSensitiveData()
    {
        await using var factory = CreateFactory($"p3-health-ready-{Guid.NewGuid():N}");
        await EnsureDatabaseAsync(factory, TestContext.Current.CancellationToken);

        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/health/ready",
            TestContext.Current.CancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.Equal("healthy", root.GetProperty("status").GetString());

        var checks = root.GetProperty("checks");
        Assert.Equal(1, checks.GetArrayLength());
        Assert.Equal("database", checks[0].GetProperty("name").GetString());
        Assert.Equal("healthy", checks[0].GetProperty("status").GetString());
        Assert.Equal("Database connectivity check passed.", checks[0].GetProperty("description").GetString());

        Assert.DoesNotContain("ConnectionStrings", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Server=", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Password", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("TrustServerCertificate", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PayloadJson", responseBody, StringComparison.OrdinalIgnoreCase);
    }

    private static WebApplicationFactory<Program> CreateFactory(string databaseName)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("environment", "Development");

                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Authentication:Mode"] = "Development",
                        ["ConnectionStrings:SqlServer"] = string.Empty,
                        ["Features:EnableSwaggerInDevelopment"] = "false",
                        ["Security:RateLimiting:Enabled"] = "false"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<CaritasDbContext>>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();
                    services.RemoveAll<CaritasDbContext>();

                    services.AddDbContext<CaritasDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName);
                    });
                });
            });
    }

    private static async Task EnsureDatabaseAsync(
        WebApplicationFactory<Program> factory,
        CancellationToken cancellationToken)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
