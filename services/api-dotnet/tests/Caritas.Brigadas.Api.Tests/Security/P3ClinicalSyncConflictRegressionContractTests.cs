using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ClinicalSyncConflictRegressionContractTests
{
    [Fact]
    public void ClinicalSyncConflictRegression_ValidatesConflictWithoutBatchAbort()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3ClinicalSyncEndToEndIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "SyncBatchProcessor_CompletesBatchWhenDuplicatePatientFolioCreatesConflict",
            "new SyncBatchProcessor(dbContext)",
            "eventsCount: 2",
            "SyncEntityType.Patient",
            "PAT-CONFLICT-001",
            "Assert.Equal(2, result.PendingEventsProcessed)",
            "Assert.Equal(1, result.AcceptedCount)",
            "Assert.Equal(0, result.RejectedCount)",
            "Assert.Equal(1, result.ConflictCount)",
            "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
            "SyncEventStatus.Accepted",
            "SyncEventStatus.Conflict",
            "patient_folio_duplicate_in_pending_batch",
            "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)",
            "Assert.Equal(1, completedBatch.AcceptedCount)",
            "Assert.Equal(0, completedBatch.RejectedCount)",
            "Assert.Equal(1, completedBatch.ConflictCount)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync conflict regression test");
    }

    [Fact]
    public void ClinicalSyncConflictRegressionBaseline_DefinesControlledConflictScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_CLINICAL_SYNC_CONFLICT_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Clinical Sync Conflict Regression Baseline",
            "duplicate patient folio detection inside the same pending batch",
            "PendingEventsProcessed equals 2",
            "AcceptedCount equals 1",
            "RejectedCount equals 0",
            "ConflictCount equals 1",
            "patient_folio_duplicate_in_pending_batch",
            "Controlled conflicts are expected domain outcomes",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync conflict regression baseline");
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
