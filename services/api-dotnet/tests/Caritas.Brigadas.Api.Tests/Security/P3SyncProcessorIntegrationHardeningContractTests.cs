using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorIntegrationHardeningContractTests
{
    [Fact]
    public void SyncBatchProcessor_KeepsTopologicalCreateOrder()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "ServiceEncounterSyncEventHandler.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "FormResponseSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "private static int GetSyncProcessingOrder(SyncEvent syncEvent)",
            "syncEvent.EntityType == SyncEntityType.Patient",
            "return 0;",
            "syncEvent.EntityType == SyncEntityType.PatientVisit",
            "return 1;",
            "syncEvent.EntityType == SyncEntityType.ServiceEncounter",
            "return 2;",
            "syncEvent.EntityType == SyncEntityType.VitalSigns",
            "return 3;",
            "syncEvent.EntityType == SyncEntityType.FormResponse",
            "return 4;",
            "syncEvent.EntityType == SyncEntityType.ConsentDocument",
            "return 5;",
            "syncEvent.EntityType == SyncEntityType.MedicalReferral",
            "return 6;",
            "syncEvent.EntityType == SyncEntityType.MedicationDelivery",
            "return 7;",
            "return 8;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor topological order");
    }

    [Fact]
    public void SyncBatchProcessor_UsesAtomicMultiKeyPendingBatchReservations()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "ServiceEncounterSyncEventHandler.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "FormResponseSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "reserved only after successful ServiceEncounter construction and reserved atomically",
            "encounterFolioReserved",
            "encounterVisitServiceKeyReserved",
            "acceptedEncounterFoliosInBatch.Remove(normalizedEncounterFolio)",
            "reserved only after successful FormResponse construction and reserved atomically",
            "formResponseIdReserved",
            "formResponseEncounterTemplateKeyReserved",
            "acceptedFormResponseIdsInBatch.Remove(formResponseId)",
            "reserved only after successful ConsentDocument construction and reserved atomically",
            "consentDocumentIdReserved",
            "consentDocumentKeyReserved",
            "acceptedConsentDocumentIdsInBatch.Remove(consentDocumentId)",
            "reserved only after successful MedicalReferral construction and reserved atomically",
            "medicalReferralIdReserved",
            "medicalReferralFolioReserved",
            "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor atomic pending-batch reservations");
    }

    [Fact]
    public void SyncProcessorContractTests_DoNotUseImplicitlyTypedEmptyArrays()
    {
        var securityTestsPath = Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "tests",
            "Caritas.Brigadas.Api.Tests",
            "Security");

        var offenders = Directory
            .GetFiles(securityTestsPath, "P3SyncProcessor*ContractTests.cs")
            .SelectMany(path => File.ReadAllLines(path)
                .Select((line, index) => new { path, line, number = index + 1 }))
            .Where(item => System.Text.RegularExpressions.Regex.IsMatch(
                item.line,
                @"var\s+\w+\s*=\s*new\[\]\s*\{\s*\};"))
            .Select(item => $"{Path.GetFileName(item.path)}:{item.number}:{item.line}")
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "P3 sync processor contract tests must not use implicitly typed empty arrays." +
            Environment.NewLine +
            string.Join(Environment.NewLine, offenders));
    }

    [Fact]
    public void IntegrationHardeningBaseline_DefinesCrossHandlerScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_INTEGRATION_HARDENING_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Integration Hardening Baseline",
            "Topological order contract",
            "Pending-batch reservation atomicity",
            "Duplicate behavior",
            "Payload privacy",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor integration hardening baseline");
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
