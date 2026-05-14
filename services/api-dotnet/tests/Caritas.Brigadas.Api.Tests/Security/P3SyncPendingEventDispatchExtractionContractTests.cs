using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncPendingEventDispatchExtractionContractTests
{
    [Fact]
    public void SyncBatchProcessor_ExtractsPendingEventDispatchFromProcessAsync()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private async Task ProcessPendingEventAsync",
            "await ProcessPendingEventAsync(",
            "PendingBatchReservationState reservationState",
            "syncEvent.MarkProcessing();",
            "TryValidateEvent(syncEvent, out var rejectionReason)",
            "await _patientSyncEventHandler.HandleAsync(",
            "await _patientVisitSyncEventHandler.HandleAsync(",
            "await _serviceEncounterSyncEventHandler.HandleAsync(",
            "await _vitalSignsSyncEventHandler.HandleAsync(",
            "await _formResponseSyncEventHandler.HandleAsync(",
            "await _consentDocumentSyncEventHandler.HandleAsync(",
            "await _medicalReferralSyncEventHandler.HandleAsync(",
            "await _medicationDeliverySyncEventHandler.HandleAsync(",
            "SkeletonConflictReason"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor pending event dispatch extraction");
    }

    [Fact]
    public void ProcessAsync_DoesNotDirectlyDispatchByEntityType()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var processAsyncStart = source.IndexOf(
            "public async Task<ProcessSyncBatchResultDto> ProcessAsync",
            StringComparison.Ordinal);

        var dispatchStart = source.IndexOf(
            "private async Task ProcessPendingEventAsync",
            StringComparison.Ordinal);

        Assert.True(processAsyncStart >= 0, "ProcessAsync was not found.");
        Assert.True(dispatchStart > processAsyncStart, "ProcessPendingEventAsync must appear after ProcessAsync.");

        var processAsyncSection = source[processAsyncStart..dispatchStart];

        var forbiddenTokens = new[]
        {
            "syncEvent.EntityType == SyncEntityType.Patient",
            "syncEvent.EntityType == SyncEntityType.PatientVisit",
            "syncEvent.EntityType == SyncEntityType.ServiceEncounter",
            "syncEvent.EntityType == SyncEntityType.VitalSigns",
            "syncEvent.EntityType == SyncEntityType.FormResponse",
            "syncEvent.EntityType == SyncEntityType.ConsentDocument",
            "syncEvent.EntityType == SyncEntityType.MedicalReferral",
            "syncEvent.EntityType == SyncEntityType.MedicationDelivery",
            "SkeletonConflictReason);"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, processAsyncSection, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PendingEventDispatchExtractionBaseline_DefinesDispatchBoundary()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PENDING_EVENT_DISPATCH_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Pending Event Dispatch Extraction Baseline",
            "ProcessAsync must call ProcessPendingEventAsync for each pending event",
            "ProcessAsync must not directly branch on SyncEntityType for handler dispatch",
            "ProcessPendingEventAsync must dispatch to the existing patient handler",
            "ProcessPendingEventAsync must dispatch to the existing medication delivery handler",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync pending event dispatch extraction baseline");
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
