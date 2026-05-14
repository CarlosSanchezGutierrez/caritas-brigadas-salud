using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3PatientVisitSyncEventHandlerExtractionContractTests
{
    [Fact]
    public void PatientVisitSyncEventHandler_OwnsPatientVisitCreateBehavior()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "PatientVisitSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "internal sealed class PatientVisitSyncEventHandler",
            "public async Task HandleAsync",
            "SyncPayloadReader.TryReadObject",
            "out CreatePatientVisitRequest? request",
            "var visit = new PatientVisit(",
            "patient_visit_operation_not_implemented",
            "patient_visit_brigade_mismatch",
            "patient_visit_patient_not_found",
            "patient_visit_brigade_not_found",
            "patient_visit_registered_by_user_not_found",
            "patient_visit_id_already_exists",
            "patient_visit_folio_duplicate_in_pending_batch",
            "patient_visit_folio_already_exists",
            "GenerateSyncVisitFolio",
            "syncEvent.Accept("
        };

        AssertRequiredTokens(source, requiredTokens, "PatientVisitSyncEventHandler");
    }

    [Fact]
    public void SyncBatchProcessor_DelegatesPatientVisitCreateToPatientVisitSyncEventHandler()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly PatientVisitSyncEventHandler _patientVisitSyncEventHandler;",
            "_patientVisitSyncEventHandler = new PatientVisitSyncEventHandler(dbContext, PayloadJsonOptions);",
            "    private async Task HandlePatientVisitEventAsync",
            "await _patientVisitSyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor patient visit handler extraction");

        var forbiddenTokens = new[]
        {
            "out CreatePatientVisitRequest? request",
            "var visit = new PatientVisit(",
            "patient_visit_folio_duplicate_in_pending_batch",
            "patient_visit_folio_already_exists",
            "patient_visit_patient_not_found",
            "patient_visit_brigade_not_found",
            "patient_visit_registered_by_user_not_found",
            "GenerateSyncVisitFolio("
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PatientVisitHandlerExtractionBaseline_DefinesSecondHandlerExtraction()
    {
        var source = File.ReadAllText(GetDocPath("P3_PATIENT_VISIT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Patient Visit Sync Event Handler Extraction Baseline",
            "PatientVisitSyncEventHandler must own patient_visit/create payload parsing",
            "SyncBatchProcessor must not directly construct PatientVisit",
            "SyncBatchProcessor must not directly parse CreatePatientVisitRequest",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 patient visit sync event handler extraction baseline");
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