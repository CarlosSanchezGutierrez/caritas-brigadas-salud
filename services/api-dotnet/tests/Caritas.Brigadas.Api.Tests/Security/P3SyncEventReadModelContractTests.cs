using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncEventReadModelContractTests
{
    [Fact]
    public void SyncEventSummaryDto_ExposesSafeEventMetadataAndExcludesPayloadJson()
    {
        var source = File.ReadAllText(GetContractPath("Sync", "SyncEventSummaryDto.cs"));

        var requiredTokens = new[]
        {
            "SyncEventSummaryDto",
            "SyncBatchId",
            "OrganizationId",
            "LocalEventId",
            "IdempotencyKey",
            "EntityType",
            "EntityId",
            "Operation",
            "Status",
            "ErrorMessage",
            "ConflictReason",
            "CreatedAtDevice",
            "ReceivedAtServer",
            "ProcessedAt"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncEventSummaryDto");

        Assert.DoesNotContain("PayloadJson", source, StringComparison.Ordinal);
    }

    [Fact]
    public void SyncBatchReadRepository_ListsEventsByOrganizationAndBatchWithoutPayloadJsonProjection()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchReadRepository.cs"));

        var requiredTokens = new[]
        {
            "ListEventsByBatchAsync",
            "_dbContext.SyncEvents",
            "syncEvent.OrganizationId == organizationId",
            "syncEvent.SyncBatchId == syncBatchId",
            "new SyncEventSummaryDto",
            "LocalEventId = syncEvent.LocalEventId",
            "IdempotencyKey = syncEvent.IdempotencyKey",
            "EntityType = syncEvent.EntityType",
            "Operation = syncEvent.Operation",
            "Status = syncEvent.Status",
            "ReceivedAtServer = syncEvent.ReceivedAtServer",
            "IsPending = syncEvent.Status == SyncEventStatus.Pending",
            "IsAccepted = syncEvent.Status == SyncEventStatus.Accepted",
            "IsRejected = syncEvent.Status == SyncEventStatus.Rejected",
            "IsConflict = syncEvent.Status == SyncEventStatus.Conflict"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchReadRepository event read model");

        Assert.DoesNotContain("PayloadJson =", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsPending = syncEvent.IsPending", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsAccepted = syncEvent.IsAccepted", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsRejected = syncEvent.IsRejected", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsConflict = syncEvent.IsConflict", source, StringComparison.Ordinal);
    }

    [Fact]
    public void SyncBatchesController_ExposesTenantScopedEventsEndpoint()
    {
        var source = File.ReadAllText(GetControllerPath("SyncBatchesController.cs"));

        var requiredTokens = new[]
        {
            "ListEventsByBatchAsync",
            "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/events",
            "Authorize(Policy = PermissionCodes.SyncBatchesRead)",
            "batch is null || batch.OrganizationId != organizationId",
            "repository.ListEventsByBatchAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchesController event endpoint");
    }

    [Fact]
    public void SyncEventReadModelBaseline_RequiresPayloadJsonExclusion()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_EVENT_READ_MODEL_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Event Read Model Baseline",
            "must not expose PayloadJson",
            "SyncEventSummaryDto must not expose",
            "ISyncBatchReadRepository must expose ListEventsByBatchAsync",
            "query SyncEvents by OrganizationId and SyncBatchId",
            "contract tests protect PayloadJson exclusion"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync event read model baseline");
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

    private static string GetContractPath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Contracts" }
                .Concat(segments)
                .ToArray());
    }

    private static string GetInfrastructurePath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Infrastructure" }
                .Concat(segments)
                .ToArray());
    }

    private static string GetControllerPath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Api", "Controllers" }
                .Concat(segments)
                .ToArray());
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