using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorDirectHandlerDispatchContractTests
{
    [Fact]
    public void SyncBatchProcessor_DispatchesDirectlyToExtractedHandlers()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "await _patientSyncEventHandler.HandleAsync(",
            "await _patientVisitSyncEventHandler.HandleAsync(",
            "await _serviceEncounterSyncEventHandler.HandleAsync(",
            "await _vitalSignsSyncEventHandler.HandleAsync(",
            "await _formResponseSyncEventHandler.HandleAsync(",
            "await _consentDocumentSyncEventHandler.HandleAsync(",
            "await _medicalReferralSyncEventHandler.HandleAsync(",
            "await _medicationDeliverySyncEventHandler.HandleAsync(",
            ".OrderBy(SyncProcessingOrder.GetOrder)",
            "var reservationState = new PendingBatchReservationState();",
            "await ProcessPendingEventAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor direct handler dispatch");

        var forbiddenTokens = new[]
        {
            "HandlePatientEventAsync",
            "HandlePatientVisitEventAsync",
            "HandleServiceEncounterEventAsync",
            "HandleVitalSignsEventAsync",
            "HandleFormResponseEventAsync",
            "HandleConsentDocumentEventAsync",
            "HandleMedicalReferralEventAsync",
            "HandleMedicationDeliveryEventAsync",
            "GetSyncProcessingOrder",
            "return SyncProcessingOrder.GetOrder(syncEvent);"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }

        var forbiddenPatterns = new[]
        {
            @"(?m)[ \t]+$",
            @"(\r?\n){4,}",
            @"(?m)^private async Task Handle[A-Za-z]+EventAsync",
            @"(?m)^private static int GetSyncProcessingOrder"
        };

        var failures = forbiddenPatterns
            .Where(pattern => Regex.IsMatch(source, pattern))
            .Select(pattern => $"Forbidden direct dispatch pattern found: {pattern}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "SyncBatchProcessor contains temporary wrapper residue." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void DirectHandlerDispatchBaseline_DefinesWrapperRemoval()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_DIRECT_HANDLER_DISPATCH_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Direct Handler Dispatch Baseline",
            "SyncBatchProcessor must dispatch patient events directly to PatientSyncEventHandler.HandleAsync",
            "SyncBatchProcessor must not contain temporary Handle*EventAsync wrappers",
            "SyncBatchProcessor must not contain GetSyncProcessingOrder",
            "P3-22O does not change handler behavior",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor direct handler dispatch baseline");
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
