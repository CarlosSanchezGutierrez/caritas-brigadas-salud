using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessEndpointApiRegressionContractTests
{
    [Fact]
    public void SyncProcessEndpointIntegrationTest_ValidatesHttpWiring()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3SyncProcessEndpointIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "P3SyncProcessEndpointIntegrationTests",
            "WebApplicationFactory<Program>",
            "Authentication:Mode",
            "Development",
            "X-Dev-User-Id",
            "X-Dev-Organization-Id",
            "X-Dev-Roles",
            "X-Dev-Permissions",
            "sync-batches.write",
            "ProcessEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
            "HttpStatusCode.Unauthorized",
            "ProcessEndpoint_WhenAuthenticatedWithSyncWritePermission_ProcessesPendingBatch",
            "/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process",
            "UseInMemoryDatabase",
            "services.RemoveAll<DbContextOptions<CaritasDbContext>>();",
            "services.RemoveAll<DbContextOptions>();",
            "services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();",
            "services.AddScoped<ISyncBatchProcessor, SyncBatchProcessor>();",
            "SyncEntityType.Patient",
            "SyncOperation.Create",
            "Assert.Equal(HttpStatusCode.OK, response.StatusCode)",
            "Sync batch processed successfully.",
            "pendingEventsProcessed",
            "acceptedCount",
            "rejectedCount",
            "conflictCount",
            "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
            "Assert.Equal(SyncEventStatus.Accepted, syncEvent.Status)",
            "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync process endpoint API regression test");
    }

    [Fact]
    public void SyncProcessEndpointController_IsNotSkeletonCopy()
    {
        var source = File.ReadAllText(GetControllerPath("SyncBatchesController.cs"));

        var requiredTokens = new[]
        {
            "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/process",
            "Authorize(Policy = PermissionCodes.SyncBatchesWrite)",
            "ISyncBatchProcessor",
            "processor.ProcessAsync(",
            "Sync batch processed successfully."
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchesController process endpoint");

        Assert.DoesNotContain("skeleton processor", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sin aplicar todavía escrituras clínicas reales", source, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SyncProcessEndpointApiRegressionBaseline_DefinesApiLevelScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESS_ENDPOINT_API_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Process Endpoint API Regression Baseline",
            "POST /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process",
            "PermissionCodes.SyncBatchesWrite",
            "401 Unauthorized",
            "Sync batch processed successfully.",
            "stale skeleton wording is removed",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync process endpoint API regression baseline");
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
