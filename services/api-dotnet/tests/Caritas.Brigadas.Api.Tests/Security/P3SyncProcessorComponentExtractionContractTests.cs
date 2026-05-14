using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorComponentExtractionContractTests
{
    [Fact]
    public void SyncProcessingOrder_OwnsTopologicalOrder()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs"));

        var requiredTokens = new[]
        {
            "internal static class SyncProcessingOrder",
            "public static int GetOrder(SyncEvent syncEvent)",
            "SyncEntityType.Patient",
            "return 0;",
            "SyncEntityType.PatientVisit",
            "return 1;",
            "SyncEntityType.ServiceEncounter",
            "return 2;",
            "SyncEntityType.VitalSigns",
            "return 3;",
            "SyncEntityType.FormResponse",
            "return 4;",
            "SyncEntityType.ConsentDocument",
            "return 5;",
            "SyncEntityType.MedicalReferral",
            "return 6;",
            "SyncEntityType.MedicationDelivery",
            "return 7;",
            "return 8;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncProcessingOrder");
    }

    [Fact]
    public void PendingBatchReservationState_OwnsReservationSets()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "PendingBatchReservationState.cs"));

        var requiredTokens = new[]
        {
            "internal sealed class PendingBatchReservationState",
            "AcceptedPatientFoliosInBatch",
            "AcceptedVisitFoliosInBatch",
            "AcceptedVitalSignsIdsInBatch",
            "AcceptedEncounterFoliosInBatch",
            "AcceptedEncounterVisitServiceKeysInBatch",
            "AcceptedFormResponseIdsInBatch",
            "AcceptedFormResponseEncounterTemplateKeysInBatch",
            "AcceptedConsentDocumentIdsInBatch",
            "AcceptedConsentDocumentKeysInBatch",
            "AcceptedMedicalReferralIdsInBatch",
            "AcceptedMedicalReferralFoliosInBatch",
            "AcceptedMedicationDeliveryIdsInBatch"
        };

        AssertRequiredTokens(source, requiredTokens, "PendingBatchReservationState");
    }

    [Fact]
    public void SyncBatchProcessor_UsesExtractedOrderAndReservationState()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs"));

        var requiredTokens = new[]
        {
            "var reservationState = new PendingBatchReservationState();",
            ".OrderBy(SyncProcessingOrder.GetOrder)",
            "return SyncProcessingOrder.GetOrder(syncEvent);",
            "reservationState.AcceptedPatientFoliosInBatch",
            "reservationState.AcceptedVisitFoliosInBatch",
            "reservationState.AcceptedVitalSignsIdsInBatch",
            "reservationState.AcceptedEncounterFoliosInBatch",
            "reservationState.AcceptedEncounterVisitServiceKeysInBatch",
            "reservationState.AcceptedFormResponseIdsInBatch",
            "reservationState.AcceptedFormResponseEncounterTemplateKeysInBatch",
            "reservationState.AcceptedConsentDocumentIdsInBatch",
            "reservationState.AcceptedConsentDocumentKeysInBatch",
            "reservationState.AcceptedMedicalReferralIdsInBatch",
            "reservationState.AcceptedMedicalReferralFoliosInBatch",
            "reservationState.AcceptedMedicationDeliveryIdsInBatch"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor component extraction");

        var forbiddenTokens = new[]
        {
            "var acceptedPatientFoliosInBatch = new HashSet",
            "var acceptedVisitFoliosInBatch = new HashSet",
            "var acceptedVitalSignsIdsInBatch = new HashSet",
            "var acceptedEncounterFoliosInBatch = new HashSet",
            "var acceptedFormResponseIdsInBatch = new HashSet",
            "var acceptedConsentDocumentIdsInBatch = new HashSet",
            "var acceptedMedicalReferralIdsInBatch = new HashSet",
            "var acceptedMedicationDeliveryIdsInBatch = new HashSet",
            ".OrderBy(SyncProcessingOrder.GetOrder)"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ComponentExtractionBaseline_DefinesFirstDecompositionStep()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_COMPONENT_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Component Extraction Baseline",
            "SyncProcessingOrder",
            "PendingBatchReservationState",
            "No domain handler is extracted in this package",
            "SyncBatchProcessor must sort pending events using SyncProcessingOrder.GetOrder",
            "SyncBatchProcessor must instantiate PendingBatchReservationState once per ProcessAsync call",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor component extraction baseline");
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