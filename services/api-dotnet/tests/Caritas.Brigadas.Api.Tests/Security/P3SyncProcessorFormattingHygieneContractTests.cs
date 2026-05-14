using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorFormattingHygieneContractTests
{
    [Fact]
    public void SyncBatchProcessor_DoesNotContainFormattingDebt()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var forbiddenPatterns = new[]
        {
            @"(?m)[ \t]+$",
            @"(?m)^private async Task Handle[A-Za-z]+EventAsync",
            @"(?m)^var\s+",
            @"(?m)^if\s*\(",
            @"(?m)^\s*}\r?\nprivate async Task",
            @"(?m)^await\s+",
            @"(?m)^return;"
        };

        var failures = forbiddenPatterns
            .Where(pattern => Regex.IsMatch(source, pattern))
            .Select(pattern => $"Forbidden formatting pattern found: {pattern}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "SyncBatchProcessor contains formatting hygiene violations." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void SyncBatchProcessor_ContainsIndentedHandlerDeclarations()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "    private async Task await _patientSyncEventHandler.HandleAsync(",
            "await _patientVisitSyncEventHandler.HandleAsync(",
            "    private async Task await _serviceEncounterSyncEventHandler.HandleAsync(",
            "    private async Task await _vitalSignsSyncEventHandler.HandleAsync(",
            "await _formResponseSyncEventHandler.HandleAsync(",
            "    private async Task await _consentDocumentSyncEventHandler.HandleAsync(",
            "await _medicalReferralSyncEventHandler.HandleAsync(",
            "await _medicationDeliverySyncEventHandler.HandleAsync(",
            ".OrderBy(SyncProcessingOrder.GetOrder)",
            "var reservationState = new PendingBatchReservationState();"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor formatting hygiene");
    }

    [Fact]
    public void ExtractedSyncHandlers_ContainPayloadReaderUsage()
    {
        var source =
            File.ReadAllText(GetInfrastructurePath("Sync", "PatientSyncEventHandler.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "PatientVisitSyncEventHandler.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "ServiceEncounterSyncEventHandler.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "VitalSignsSyncEventHandler.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "FormResponseSyncEventHandler.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "ConsentDocumentSyncEventHandler.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "MedicalReferralSyncEventHandler.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "MedicationDeliverySyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "SyncPayloadReader.TryReadObject",
            "PatientSyncEventHandler",
            "PatientVisitSyncEventHandler",
            "ServiceEncounterSyncEventHandler",
            "VitalSignsSyncEventHandler",
            "FormResponseSyncEventHandler",
            "ConsentDocumentSyncEventHandler",
            "MedicalReferralSyncEventHandler",
            "MedicationDeliverySyncEventHandler"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 extracted sync handler formatting hygiene");
    }

    [Fact]
    public void FormattingHygieneBaseline_DefinesZeroDebtFormattingRules()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_FORMATTING_HYGIENE_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Formatting Hygiene Baseline",
            "SyncBatchProcessor must not contain trailing whitespace",
            "SyncBatchProcessor handler methods must not start at column 1",
            "SyncBatchProcessor must not contain unindented local var declarations at column 1",
            "SyncBatchProcessor must not contain unindented if statements at column 1",
            "SyncBatchProcessor must not contain method declarations glued directly after a closing brace",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor formatting hygiene baseline");
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
