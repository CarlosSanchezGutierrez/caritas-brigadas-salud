using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorPatientVisitHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesPatientVisitCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "PatientVisitSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "HandlePatientVisitEventAsync",
            "syncEvent.EntityType == SyncEntityType.PatientVisit",
            "syncEvent.Operation != SyncOperation.Create",
            "patient_visit_operation_not_implemented",
            "out CreatePatientVisitRequest? request",
            "new PatientVisit(",
            "_dbContext.PatientVisits.Add(visit)",
            "syncEvent.Accept(",
            "visit.Id",
            "patient_visit_patient_not_found",
            "patient_visit_brigade_not_found",
            "patient_visit_brigade_mismatch",
            "patient_visit_registered_by_user_not_found",
            "patient_visit_folio_already_exists",
            "patient_visit_folio_duplicate_in_pending_batch",
            "acceptedVisitFoliosInBatch",
            "acceptedVisitFoliosInBatch.Contains(normalizedVisitFolio)",
            "!acceptedVisitFoliosInBatch.Add(normalizedVisitFolio)",
            "GenerateSyncVisitFolio",
            "GetSyncProcessingOrder",
            ".OrderBy(SyncProcessingOrder.GetOrder)",
            "pendingEvents = pendingEvents",
            "return 0;",
            "return 1;",
            "return 2;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor patient visit handler");

        var forbiddenTokens = System.Array.Empty<string>();

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PatientVisitHandlerBaseline_DefinesPatientVisitOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_PATIENT_VISIT_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Patient Visit Handler Baseline",
            "EntityType: patient_visit",
            "Operation: create",
            "parse PayloadJson as CreatePatientVisitRequest",
            "validate PatientId belongs to the same OrganizationId",
            "validate PatientId can be found either in persisted Patients or in Patients staged in the same DbContext",
            "process patient create events before patient_visit create events",
            "patient_visit update is not implemented in P3-14",
            "patient_visit void/cancel is not implemented in P3-14",
            "processor must not create service encounters, vital signs, forms, documents, referrals, or medication deliveries in P3-14",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor patient visit handler baseline");
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