using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncBatchEventIntakeContractTests
{
[Fact]
public void CreateSyncBatchRequest_SupportsClientInstanceFallback()
{
var source = File.ReadAllText(GetContractPath("Sync", "CreateSyncBatchRequest.cs"));

    var requiredTokens = new[]
    {
        "ClientInstanceId",
        "[MaxLength(150)]",
        "PayloadJson",
        "EventsCount"
    };

    AssertRequiredTokens(source, requiredTokens, "CreateSyncBatchRequest");
}

[Fact]
public void SyncBatchWriteRepository_ParsesPayloadAndCreatesPendingSyncEvents()
{
    var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchWriteRepository.cs"));

    var requiredTokens = new[]
    {
        "ExtractSyncPayloadEvents",
        "ParseSyncPayloadEvent",
        "GetEventElements",
        "BuildIdempotencyKey",
        "Client instance id is required when device id is not provided.",
        "_dbContext.SyncEvents",
        "_dbContext.SyncEvents.AddRange(newEvents)",
        "existingKeys",
        "existingKeySet",
        "Payload contains duplicate sync event idempotency keys.",
        "new SyncEvent(",
        "localEventId: item.LocalEventId",
        "entityType: item.EntityType",
        "operation: item.Operation",
        "payloadJson: item.PayloadJson",
        "idempotencyKey: BuildIdempotencyKey("
    };

    AssertRequiredTokens(source, requiredTokens, "SyncBatchWriteRepository intake");
}

[Fact]
public void SyncBatchEventIntakeBaseline_RequiresSafeStagingOnly()
{
    var source = File.ReadAllText(GetDocPath("P3_SYNC_BATCH_EVENT_INTAKE_BASELINE.md"));

    var requiredTokens = new[]
    {
        "P3 Sync Batch Event Intake Baseline",
        "P3-10 only stages events",
        "must not yet apply clinical changes",
        "ClientInstanceId is required when DeviceId is not provided",
        "duplicate idempotency keys inside the same payload must be rejected",
        "duplicate idempotency keys already stored for the same organization must not create new SyncEvent records",
        "SyncBatchWriteRepository does not apply clinical domain writes"
    };

    AssertRequiredTokens(source, requiredTokens, "P3 sync batch event intake baseline");
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
