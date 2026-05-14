using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorPostExtractionHygieneContractTests
{
    [Fact]
    public void SyncBatchProcessor_DoesNotContainPostExtractionResidue()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var forbiddenTokens = new[]
        {
            "using Caritas.Brigadas.Contracts.Patients;",
            "using Caritas.Brigadas.Contracts.PatientVisits;",
            "using Caritas.Brigadas.Contracts.ServiceEncounters;",
            "using Caritas.Brigadas.Contracts.FormResponses;",
            "using Caritas.Brigadas.Contracts.VitalSigns;",
            "GenerateSyncPatientFolio",
            "private static Sex ParseSex",
            "return Sex.NotSpecified;",
            "\"male\" or \"masculino\" or \"m\"",
            "\"female\" or \"femenino\" or \"f\""
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }

        var forbiddenPatterns = new[]
        {
            @"(?m)[ \t]+$",
            @"(\r?\n){4,}",
            @"(?m)^\s*}\r?\n    private async Task Handle",
            @"(?m)^private async Task Handle[A-Za-z]+EventAsync",
            @"(?m)^var\s+",
            @"(?m)^if\s*\(",
            @"(?m)^await\s+",
            @"(?m)^return;"
        };

        var failures = forbiddenPatterns
            .Where(pattern => Regex.IsMatch(source, pattern))
            .Select(pattern => $"Forbidden post-extraction hygiene pattern found: {pattern}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "SyncBatchProcessor contains post-extraction hygiene violations." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void SyncBatchProcessor_StillDelegatesToAllExtractedHandlers()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly PatientSyncEventHandler _patientSyncEventHandler;",
            "private readonly PatientVisitSyncEventHandler _patientVisitSyncEventHandler;",
            "private readonly ServiceEncounterSyncEventHandler _serviceEncounterSyncEventHandler;",
            "private readonly VitalSignsSyncEventHandler _vitalSignsSyncEventHandler;",
            "private readonly FormResponseSyncEventHandler _formResponseSyncEventHandler;",
            "private readonly ConsentDocumentSyncEventHandler _consentDocumentSyncEventHandler;",
            "private readonly MedicalReferralSyncEventHandler _medicalReferralSyncEventHandler;",
            "private readonly MedicationDeliverySyncEventHandler _medicationDeliverySyncEventHandler;",
            ".OrderBy(SyncProcessingOrder.GetOrder)",
            "var reservationState = new PendingBatchReservationState();",
            "TryValidateEvent(syncEvent, out var rejectionReason)",
            "JsonDocument.Parse(syncEvent.PayloadJson)"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor post-extraction hygiene");
    }

    [Fact]
    public void PostExtractionHygieneBaseline_DefinesNoBehaviorChangeCleanup()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_POST_EXTRACTION_HYGIENE_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Post-Extraction Hygiene Baseline",
            "SyncBatchProcessor must not contain stale request contract usings for extracted handlers",
            "SyncBatchProcessor must not contain GenerateSyncPatientFolio",
            "SyncBatchProcessor must not contain ParseSex",
            "P3-22N does not remove temporary compatibility wrappers",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor post-extraction hygiene baseline");
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
