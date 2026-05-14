using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorComponentExtractionContractTests
{
    [Fact]
    public void SyncBatchProcessor_UsesExtractedOrderAndReservationStateWithDirectHandlerDispatch()
    {
        var processor = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));
        var order = File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs"));
        var reservationState = File.ReadAllText(GetInfrastructurePath("Sync", "PendingBatchReservationState.cs"));

        var requiredProcessorTokens = new[]
        {
            ".OrderBy(SyncProcessingOrder.GetOrder)",
            "var reservationState = new PendingBatchReservationState();",
            "await _patientSyncEventHandler.HandleAsync(",
            "await _patientVisitSyncEventHandler.HandleAsync(",
            "await _serviceEncounterSyncEventHandler.HandleAsync(",
            "await _vitalSignsSyncEventHandler.HandleAsync(",
            "await _formResponseSyncEventHandler.HandleAsync(",
            "await _consentDocumentSyncEventHandler.HandleAsync(",
            "await _medicalReferralSyncEventHandler.HandleAsync(",
            "await _medicationDeliverySyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(processor, requiredProcessorTokens, "SyncBatchProcessor component extraction");

        var requiredOrderTokens = new[]
        {
            "internal static class SyncProcessingOrder",
            "public static int GetOrder(SyncEvent syncEvent)",
            "SyncEntityType.Patient",
            "SyncEntityType.PatientVisit",
            "SyncEntityType.ServiceEncounter",
            "SyncEntityType.VitalSigns",
            "SyncEntityType.FormResponse",
            "SyncEntityType.ConsentDocument",
            "SyncEntityType.MedicalReferral",
            "SyncEntityType.MedicationDelivery"
        };

        AssertRequiredTokens(order, requiredOrderTokens, "SyncProcessingOrder");

        var requiredReservationTokens = new[]
        {
            "internal sealed class PendingBatchReservationState",
            "AcceptedPatientFoliosInBatch",
            "AcceptedVisitFoliosInBatch",
            "AcceptedEncounterFoliosInBatch",
            "AcceptedEncounterVisitServiceKeysInBatch",
            "AcceptedVitalSignsIdsInBatch",
            "AcceptedFormResponseIdsInBatch",
            "AcceptedFormResponseEncounterTemplateKeysInBatch",
            "AcceptedConsentDocumentIdsInBatch",
            "AcceptedConsentDocumentKeysInBatch",
            "AcceptedMedicalReferralIdsInBatch",
            "AcceptedMedicalReferralFoliosInBatch",
            "AcceptedMedicationDeliveryIdsInBatch"
        };

        AssertRequiredTokens(reservationState, requiredReservationTokens, "PendingBatchReservationState");

        var forbiddenProcessorTokens = new[]
        {
            "HandlePatientEventAsync",
            "HandlePatientVisitEventAsync",
            "HandleServiceEncounterEventAsync",
            "HandleVitalSignsEventAsync",
            "HandleFormResponseEventAsync",
            "HandleConsentDocumentEventAsync",
            "HandleMedicalReferralEventAsync",
            "HandleMedicationDeliveryEventAsync",
            "private static int GetSyncProcessingOrder",
            "return SyncProcessingOrder.GetOrder(syncEvent);"
        };

        foreach (var token in forbiddenProcessorTokens)
        {
            Assert.DoesNotContain(token, processor, StringComparison.Ordinal);
        }
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