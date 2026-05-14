using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ClinicalSyncInvalidPayloadRegressionContractTests
{
    [Fact]
    public void ClinicalSyncInvalidPayloadRegression_ValidatesRejectedPayloadWithoutBatchAbort()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3ClinicalSyncEndToEndIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "SyncBatchProcessor_CompletesBatchWhenInvalidPayloadIsRejected",
            "new SyncBatchProcessor(dbContext)",
            "eventsCount: 2",
            "SyncEntityType.Patient",
            "PAT-REJECTED-001",
            "002-patient-invalid-json",
            "p3-rejected-invalid-json",
            "Assert.Equal(2, result.PendingEventsProcessed)",
            "Assert.Equal(1, result.AcceptedCount)",
            "Assert.Equal(1, result.RejectedCount)",
            "Assert.Equal(0, result.ConflictCount)",
            "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
            "SyncEventStatus.Accepted",
            "SyncEventStatus.Rejected",
            "rejectedEvent.ErrorMessage",
            "Sync event payload JSON is invalid.",
            "Assert.Equal(SyncBatchStatus.CompletedWithErrors, completedBatch.Status)",
            "Assert.Equal(1, completedBatch.AcceptedCount)",
            "Assert.Equal(1, completedBatch.RejectedCount)",
            "Assert.Equal(0, completedBatch.ConflictCount)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync invalid payload regression test");
    }

    [Fact]
    public void ClinicalSyncInvalidPayloadRegressionBaseline_DefinesRejectedPayloadScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_CLINICAL_SYNC_INVALID_PAYLOAD_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Clinical Sync Invalid Payload Regression Baseline",
            "malformed sync payload JSON rejects only the invalid event",
            "PendingEventsProcessed equals 2",
            "AcceptedCount equals 1",
            "RejectedCount equals 1",
            "ConflictCount equals 0",
            "rejectedEvent.ErrorMessage",
            "Sync event payload JSON is invalid.",
            "Malformed payload JSON is a controlled rejected input",
            "completed_with_errors",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync invalid payload regression baseline");
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
