using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3PatientSyncEventHandlerExtractionContractTests
{
    [Fact]
    public void PatientSyncEventHandler_OwnsPatientCreateBehavior()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "PatientSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "internal sealed class PatientSyncEventHandler",
            "public async Task HandleAsync",
            "SyncPayloadReader.TryReadObject",
            "out CreatePatientRequest? request",
            "var patient = new Patient(",
            "patient.UpdateSensitiveIdentifiers(",
            "patient.UpdateLocation(",
            "patient.MarkAsMigrant();",
            "patient.MarkAsPartialRecord(",
            "patient.UpdateAdminNotes(",
            "patient_folio_duplicate_in_pending_batch",
            "patient_folio_already_exists",
            "syncEvent.Accept("
        };

        AssertRequiredTokens(source, requiredTokens, "PatientSyncEventHandler");
    }

    [Fact]
    public void SyncBatchProcessor_DelegatesPatientCreateToPatientSyncEventHandler()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly PatientSyncEventHandler _patientSyncEventHandler;",
            "_patientSyncEventHandler = new PatientSyncEventHandler(dbContext, PayloadJsonOptions);",
            "await _patientSyncEventHandler.HandleAsync(",
            "await _patientSyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor patient handler extraction");

        var forbiddenTokens = new[]
        {
            "var patient = new Patient(",
            "patient.UpdateSensitiveIdentifiers(",
            "patient.UpdateLocation(",
            "patient.MarkAsMigrant();",
            "patient.MarkAsPartialRecord(",
            "patient.UpdateAdminNotes(",
            "out CreatePatientRequest? request"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PatientHandlerExtractionBaseline_DefinesFirstHandlerExtraction()
    {
        var source = File.ReadAllText(GetDocPath("P3_PATIENT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Patient Sync Event Handler Extraction Baseline",
            "PatientSyncEventHandler must own patient/create payload parsing",
            "SyncBatchProcessor must not directly construct Patient",
            "SyncBatchProcessor must not directly parse CreatePatientRequest",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 patient sync event handler extraction baseline");
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
