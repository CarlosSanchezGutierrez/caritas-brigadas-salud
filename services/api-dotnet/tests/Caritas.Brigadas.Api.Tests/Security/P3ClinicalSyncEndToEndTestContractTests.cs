using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ClinicalSyncEndToEndTestContractTests
{
    [Fact]
    public void ClinicalSyncEndToEndIntegrationTest_ProcessesEightPrimaryEvents()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3ClinicalSyncEndToEndIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "P3ClinicalSyncEndToEndIntegrationTests",
            "SyncBatchProcessor_ProcessesCompleteClinicalOfflineBatchEndToEnd",
            "UseInMemoryDatabase",
            "CaritasDbContext",
            "new SyncBatchProcessor(dbContext)",
            "SyncEntityType.Patient",
            "SyncEntityType.PatientVisit",
            "SyncEntityType.ServiceEncounter",
            "SyncEntityType.VitalSigns",
            "SyncEntityType.FormResponse",
            "SyncEntityType.ConsentDocument",
            "SyncEntityType.MedicalReferral",
            "SyncEntityType.MedicationDelivery",
            "Assert.Equal(8, result.PendingEventsProcessed)",
            "Assert.Equal(8, result.AcceptedCount)",
            "Assert.Equal(0, result.RejectedCount)",
            "Assert.Equal(0, result.ConflictCount)",
            "dbContext.BrigadeServices.Add(new BrigadeService",
            "dbContext.Patients.CountAsync",
            "dbContext.PatientVisits.CountAsync",
            "dbContext.ServiceEncounters.CountAsync",
            "dbContext.VitalSignsRecords.CountAsync",
            "dbContext.FormResponses.CountAsync",
            "dbContext.ConsentDocuments.CountAsync",
            "dbContext.MedicalReferrals.CountAsync",
            "dbContext.MedicationDeliveries.CountAsync",
            "Assert.Equal(SyncEventStatus.Accepted, syncEvent.Status)",
            "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync end-to-end integration test");
    }

    [Fact]
    public void ClinicalSyncEndToEndBaseline_DefinesProcessorLevelIntegrationScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_CLINICAL_SYNC_END_TO_END_TEST_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Clinical Sync End-to-End Test Baseline",
            "patient;",
            "patient_visit;",
            "service_encounter;",
            "vital_signs;",
            "form_response;",
            "consent_document;",
            "medical_referral;",
            "medication_delivery.",
            "PendingEventsProcessed equals 8",
            "AcceptedCount equals 8",
            "RejectedCount equals 0",
            "ConflictCount equals 0",
            "BrigadeService",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 clinical sync end-to-end test baseline");
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
