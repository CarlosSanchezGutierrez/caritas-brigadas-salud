using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ClinicalSyncIdempotencyRegressionContractTests
{
    [Fact]
    public void ClinicalSyncIdempotencyRegression_ValidatesAlreadyCompletedNoOp()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3ClinicalSyncEndToEndIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "SyncBatchProcessor_ReturnsAlreadyCompletedWithoutDuplicatingClinicalRows",
            "SeedCompleteClinicalBatchAsync",
            "var firstResult = await processor.ProcessAsync",
            "var secondResult = await processor.ProcessAsync",
            "AssertCompletedClinicalBatchAsync",
            "Assert.Equal(0, secondResult.PendingEventsProcessed)",
            "Assert.Equal(8, secondResult.AcceptedCount)",
            "Assert.Equal(0, secondResult.RejectedCount)",
            "Assert.Equal(0, secondResult.ConflictCount)",
            "Assert.Equal(\"Sync batch was already completed.\", secondResult.Message)",
            "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.PatientVisits.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.ServiceEncounters.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.VitalSignsRecords.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.FormResponses.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.ConsentDocuments.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.MedicalReferrals.CountAsync(cancellationToken))",
            "Assert.Equal(1, await dbContext.MedicationDeliveries.CountAsync(cancellationToken))",
            "Assert.Equal(8, syncEvents.Length)",
            "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync idempotency regression test");
    }

    [Fact]
    public void ClinicalSyncIdempotencyRegressionBaseline_DefinesAlreadyCompletedScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_CLINICAL_SYNC_IDEMPOTENCY_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Clinical Sync Idempotency Regression Baseline",
            "processing an already completed sync batch is idempotent",
            "process the same SyncBatch a second time",
            "PendingEventsProcessed equals 0",
            "AcceptedCount remains 8",
            "Sync batch was already completed.",
            "Already completed batches are immutable",
            "safe no-op",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync idempotency regression baseline");
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
