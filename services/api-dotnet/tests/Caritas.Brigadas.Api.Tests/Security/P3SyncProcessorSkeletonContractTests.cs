using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorSkeletonContractTests
{
    [Fact]
    public void SyncBatchProcessor_UsesSafeSkeletonTransitionsWithoutClinicalWrites()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "ISyncBatchProcessor",
            "ProcessAsync",
            "batch.MarkProcessing()",
            "syncEvent.Status == SyncEventStatus.Pending",
            "syncEvent.MarkProcessing()",
            "TryValidateEvent",
            "SyncEntityType.IsAllowed",
            "SyncOperation.IsAllowed",
            "JsonDocument.Parse(syncEvent.PayloadJson)",
            "syncEvent.Reject(",
            "syncEvent.MarkConflict(",
            "SkeletonConflictReason",
            "batch.Complete("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor skeleton");

        var forbiddenTokens = new[]
        {
            "_dbContext.Patients.Add",
            "_dbContext.PatientVisits.Add",
            "_dbContext.ServiceEncounters.Add",
            "_dbContext.VitalSignsRecords.Add",
            "_dbContext.FormResponses.Add",
            "_dbContext.ConsentDocuments.Add",
            "_dbContext.MedicalReferrals.Add",
            "_dbContext.MedicationDeliveries.Add",
            "syncEvent.Accept("
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void SyncBatchesController_ExposesTenantScopedProcessorEndpoint()
    {
        var source = File.ReadAllText(GetControllerPath("SyncBatchesController.cs"));

        var requiredTokens = new[]
        {
            "ProcessAsync",
            "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/process",
            "Authorize(Policy = PermissionCodes.SyncBatchesWrite)",
            "GetService<ISyncBatchProcessor>",
            "processor.ProcessAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchesController process endpoint");
    }

    [Fact]
    public void ProcessSyncBatchResultDto_DoesNotExposePayloadJson()
    {
        var source = File.ReadAllText(GetContractPath("Sync", "ProcessSyncBatchResultDto.cs"));

        var requiredTokens = new[]
        {
            "ProcessSyncBatchResultDto",
            "SyncBatchSummaryDto Batch",
            "PendingEventsProcessed",
            "AcceptedCount",
            "RejectedCount",
            "ConflictCount",
            "Completed",
            "Message"
        };

        AssertRequiredTokens(source, requiredTokens, "ProcessSyncBatchResultDto");

        Assert.DoesNotContain("PayloadJson", source, StringComparison.Ordinal);
    }

    [Fact]
    public void SyncProcessorSkeletonBaseline_DefinesNoClinicalWrites()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_SKELETON_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Skeleton Baseline",
            "must not accept raw payload in the process request",
            "must not expose PayloadJson in the response",
            "mark valid pending events as conflict because domain handlers are not implemented yet",
            "must not",
            "create Patient records",
            "create VitalSignsRecord records",
            "accept events as applied clinical writes",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor skeleton baseline");
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