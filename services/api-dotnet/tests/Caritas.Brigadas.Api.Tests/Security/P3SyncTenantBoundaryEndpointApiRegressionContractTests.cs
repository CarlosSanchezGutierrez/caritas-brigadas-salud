using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncTenantBoundaryEndpointApiRegressionContractTests
{
    [Fact]
    public void SyncTenantBoundaryEndpointIntegrationTest_ValidatesCrossTenantNotFoundAndNoProcessing()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3SyncTenantBoundaryEndpointIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "P3SyncTenantBoundaryEndpointIntegrationTests",
            "WebApplicationFactory<Program>",
            "GetByIdEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundWithoutLeakingPayload",
            "ProcessEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundAndDoesNotProcess",
            "/api/v1/organizations/{otherOrganizationId}/sync-batches/{syncBatchId}",
            "/api/v1/organizations/{otherOrganizationId}/sync-batches/{syncBatchId}/process",
            "Authentication:Mode",
            "Development",
            "X-Dev-Organization-Id",
            "sync-batches.read",
            "sync-batches.write",
            "UseInMemoryDatabase",
            "services.RemoveAll<DbContextOptions<CaritasDbContext>>();",
            "services.RemoveAll<DbContextOptions>();",
            "services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();",
            "services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();",
            "services.AddScoped<ISyncBatchProcessor, SyncBatchProcessor>();",
            "HttpStatusCode.NotFound",
            "Sync batch was not found.",
            "TenantBoundarySensitiveNameShouldNotLeak",
            "TenantBoundarySensitivePhoneShouldNotLeak",
            "Assert.DoesNotContain(\"payloadJson\", responseBody, StringComparison.OrdinalIgnoreCase)",
            "Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken))",
            "Assert.Equal(SyncBatchStatus.Received, batch.Status)",
            "Assert.Equal(0, batch.AcceptedCount)",
            "Assert.Equal(0, batch.RejectedCount)",
            "Assert.Equal(0, batch.ConflictCount)",
            "Assert.Equal(SyncEventStatus.Pending, syncEvent.Status)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync tenant boundary endpoint API regression test");
    }

    [Fact]
    public void SyncTenantBoundaryController_UsesNotFoundForCrossTenantRoutes()
    {
        var source = File.ReadAllText(GetControllerPath("SyncBatchesController.cs"));

        var requiredTokens = new[]
        {
            "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}",
            "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/process",
            "Authorize(Policy = PermissionCodes.SyncBatchesRead)",
            "Authorize(Policy = PermissionCodes.SyncBatchesWrite)",
            "Sync batch was not found."
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchesController tenant boundary endpoints");
    }

    [Fact]
    public void SyncTenantBoundaryEndpointApiRegressionBaseline_DefinesTenantBoundaryScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_TENANT_BOUNDARY_ENDPOINT_API_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Tenant Boundary Endpoint API Regression Baseline",
            "GET /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}",
            "POST /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process",
            "PermissionCodes.SyncBatchesRead",
            "PermissionCodes.SyncBatchesWrite",
            "Tenant boundary rule",
            "404 NotFound",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync tenant boundary endpoint API regression baseline");
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
