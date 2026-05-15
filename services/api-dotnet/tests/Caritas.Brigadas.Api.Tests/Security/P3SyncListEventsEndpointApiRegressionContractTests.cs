using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncListEventsEndpointApiRegressionContractTests
{
    [Fact]
    public void SyncListEventsEndpointIntegrationTest_ValidatesNoPayloadJsonLeakage()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3SyncListEventsEndpointIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "P3SyncListEventsEndpointIntegrationTests",
            "WebApplicationFactory<Program>",
            "ListEventsEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
            "HttpStatusCode.Unauthorized",
            "ListEventsEndpoint_WhenAuthenticatedWithSyncReadPermission_ReturnsEventsWithoutPayloadJson",
            "ListEventsEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFound",
            "/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events",
            "Authentication:Mode",
            "Development",
            "X-Dev-User-Id",
            "X-Dev-Organization-Id",
            "X-Dev-Roles",
            "X-Dev-Permissions",
            "sync-batches.read",
            "UseInMemoryDatabase",
            "services.RemoveAll<DbContextOptions<CaritasDbContext>>();",
            "services.RemoveAll<DbContextOptions>();",
            "services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();",
            "services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();",
            "SensitiveFirstNameShouldNotLeak",
            "Assert.DoesNotContain(\"payloadJson\", responseBody, StringComparison.OrdinalIgnoreCase)",
            "Assert.False(item.TryGetProperty(\"payloadJson\", out _))",
            "Assert.False(item.TryGetProperty(\"payload\", out _))",
            "Assert.Equal(HttpStatusCode.OK, response.StatusCode)",
            "Assert.Equal(HttpStatusCode.NotFound, response.StatusCode)",
            "Sync batch was not found.",
            "Assert.Equal(SyncEventStatus.Pending, item.GetProperty(\"status\").GetString())",
            "Assert.Equal(SyncEventStatus.Pending, syncEvent.Status)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync list events endpoint API regression test");
    }

    [Fact]
    public void SyncListEventsController_UsesReadRepositoryAndReadPermission()
    {
        var source = File.ReadAllText(GetControllerPath("SyncBatchesController.cs"));

        var requiredTokens = new[]
        {
            "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/events",
            "Authorize(Policy = PermissionCodes.SyncBatchesRead)",
            "ISyncBatchReadRepository",
            "repository.ListEventsByBatchAsync(",
            "Sync batch was not found."
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchesController list events endpoint");
    }

    [Fact]
    public void SyncEventSummaryDto_DoesNotExposePayloadJson()
    {
        var source = File.ReadAllText(GetContractPath("SyncEventSummaryDto.cs"));

        Assert.DoesNotContain("PayloadJson", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Payload", source, StringComparison.Ordinal);
    }

    [Fact]
    public void SyncBatchReadRepository_DoesNotProjectPayloadJsonIntoEventSummary()
    {
        var source = File.ReadAllText(GetInfrastructurePath("SyncBatchReadRepository.cs"));

        var projectionStart = source.IndexOf("new SyncEventSummaryDto", StringComparison.Ordinal);

        Assert.True(projectionStart >= 0, "SyncEventSummaryDto projection was not found.");

        var projection = source[projectionStart..];

        Assert.DoesNotContain("PayloadJson", projection, StringComparison.Ordinal);
    }

    [Fact]
    public void SyncListEventsEndpointApiRegressionBaseline_DefinesPrivacyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_LIST_EVENTS_ENDPOINT_API_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync List Events Endpoint API Regression Baseline",
            "GET /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events",
            "PermissionCodes.SyncBatchesRead",
            "401 Unauthorized",
            "payloadJson",
            "sensitive payload values",
            "Tenant boundary rule",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync list events endpoint API regression baseline");
    }

    private static void AssertRequiredTokens(
        string source,
        IReadOnlyCollection<string> requiredTokens,
        string label)
    {
        var failures = requiredTokens
            .Where(token => !source.Contains(token, StringComparison.Ordinal))
            .Select(token => $"{label} is missing required token: {token}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            $"{label} is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetIntegrationTestPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "tests",
            "Caritas.Brigadas.Api.Tests",
            "Integration",
            fileName);
    }

    private static string GetControllerPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            fileName);
    }

    private static string GetContractPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Contracts",
            "Sync",
            fileName);
    }

    private static string GetInfrastructurePath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Infrastructure",
            "Sync",
            fileName);
    }

    private static string GetDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "backend",
            fileName);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root with .git directory was not found.");
    }
}
