using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncCreateBatchEndpointApiRegressionContractTests
{
    [Fact]
    public void SyncCreateBatchEndpointIntegrationTest_ValidatesHttpIntakeWiring()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3SyncCreateBatchEndpointIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "P3SyncCreateBatchEndpointIntegrationTests",
            "WebApplicationFactory<Program>",
            "CreateEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
            "HttpStatusCode.Unauthorized",
            "CreateEndpoint_WhenAuthenticatedWithSyncWritePermission_CreatesBatchAndEvents",
            "/api/v1/organizations/{organizationId}/sync-batches",
            "Authentication:Mode",
            "Development",
            "X-Dev-User-Id",
            "X-Dev-Organization-Id",
            "X-Dev-Roles",
            "X-Dev-Permissions",
            "sync-batches.write",
            "UseInMemoryDatabase",
            "services.RemoveAll<DbContextOptions<CaritasDbContext>>();",
            "services.RemoveAll<DbContextOptions>();",
            "services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();",
            "services.AddScoped<ISyncBatchWriteRepository, SyncBatchWriteRepository>();",
            "CreateSyncBatchRequest",
            "PayloadJson",
            "SyncEntityType.Patient",
            "SyncOperation.Create",
            "Assert.Equal(HttpStatusCode.Created, response.StatusCode)",
            "Assert.NotNull(response.Headers.Location)",
            "Sync batch received successfully.",
            "eventsCount",
            "received",
            "isCompleted",
            "Assert.Equal(1, await dbContext.SyncBatches.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.SyncEvents.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken))",
            "Assert.Equal(SyncBatchStatus.Received, batch.Status)",
            "Assert.Equal(SyncEventStatus.Pending, syncEvent.Status)",
            "syncEvent.IdempotencyKey"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync create batch endpoint API regression test");
    }

    [Fact]
    public void SyncCreateBatchController_UsesWriteRepositoryAndSuccessMessage()
    {
        var source = File.ReadAllText(GetControllerPath("SyncBatchesController.cs"));

        var requiredTokens = new[]
        {
            "api/v1/organizations/{organizationId:guid}/sync-batches",
            "Authorize(Policy = PermissionCodes.SyncBatchesWrite)",
            "ISyncBatchWriteRepository",
            "repository.CreateAsync(",
            "Sync batch received successfully."
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchesController create endpoint");
    }

    [Fact]
    public void SyncCreateBatchEndpointApiRegressionBaseline_DefinesApiLevelIntakeScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_CREATE_BATCH_ENDPOINT_API_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Create Batch Endpoint API Regression Baseline",
            "POST /api/v1/organizations/{organizationId}/sync-batches",
            "PermissionCodes.SyncBatchesWrite",
            "401 Unauthorized",
            "HTTP 201 Created",
            "Sync batch received successfully.",
            "Create sync batch endpoint is intake only",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync create batch endpoint API regression baseline");
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
