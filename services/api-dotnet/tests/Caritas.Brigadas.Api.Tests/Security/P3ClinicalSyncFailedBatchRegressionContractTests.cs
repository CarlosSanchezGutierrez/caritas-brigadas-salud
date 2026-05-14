using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ClinicalSyncFailedBatchRegressionContractTests
{
    [Fact]
    public void ClinicalSyncFailedBatchRegression_ValidatesFailedBatchCannotBeProcessed()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3ClinicalSyncEndToEndIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "SyncBatchProcessor_ThrowsWhenFailedBatchIsProcessed",
            "syncBatch.Fail(",
            "new SyncBatchProcessor(dbContext)",
            "Assert.ThrowsAsync<InvalidOperationException>",
            "Assert.Equal(\"Failed sync batch cannot be processed.\", exception.Message)",
            "Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.PatientVisits.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.ServiceEncounters.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.VitalSignsRecords.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.FormResponses.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.ConsentDocuments.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.MedicalReferrals.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.MedicationDeliveries.CountAsync(cancellationToken))",
            "Assert.Equal(0, await dbContext.SyncEvents.CountAsync(cancellationToken))",
            "Assert.Equal(SyncBatchStatus.Failed, failedBatch.Status)",
            "Assert.Equal(0, failedBatch.AcceptedCount)",
            "Assert.Equal(0, failedBatch.RejectedCount)",
            "Assert.Equal(0, failedBatch.ConflictCount)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync failed batch regression test");
    }

    [Fact]
    public void ClinicalSyncFailedBatchRegressionBaseline_DefinesFailedBatchScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_CLINICAL_SYNC_FAILED_BATCH_REGRESSION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Clinical Sync Failed Batch Regression Baseline",
            "failed sync batches cannot be processed",
            "mark the SyncBatch as failed using SyncBatch.Fail",
            "InvalidOperationException",
            "Failed sync batch cannot be processed.",
            "Failed batches are terminal",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync failed batch regression baseline");
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
