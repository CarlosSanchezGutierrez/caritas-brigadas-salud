using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorServiceEncounterHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesServiceEncounterCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "HandleServiceEncounterEventAsync",
            "syncEvent.EntityType == SyncEntityType.ServiceEncounter",
            "syncEvent.Operation != SyncOperation.Create",
            "service_encounter_operation_not_implemented",
            "JsonSerializer.Deserialize<CreateServiceEncounterRequest>",
            "new ServiceEncounter(",
            "_dbContext.ServiceEncounters.Add(encounter)",
            "syncEvent.Accept(",
            "encounter.Id",
            "service_encounter_visit_not_found",
            "service_encounter_brigade_mismatch",
            "service_encounter_service_not_found",
            "service_encounter_service_inactive",
            "service_encounter_service_not_available_for_brigade",
            "service_encounter_provider_user_not_found",
            "service_encounter_folio_already_exists",
            "service_encounter_folio_duplicate_in_pending_batch",
            "service_encounter_duplicate_visit_service",
            "service_encounter_duplicate_visit_service_in_pending_batch",
            "acceptedEncounterFoliosInBatch",
            "acceptedEncounterVisitServiceKeysInBatch",
            "GenerateSyncEncounterFolio",
            "private static string GenerateSyncEncounterFolio",
            "return 2;",
            "return 3;",
            "return 4;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor service encounter handler");

        var forbiddenTokens = System.Array.Empty<string>();

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ServiceEncounterHandlerBaseline_DefinesServiceEncounterOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_SERVICE_ENCOUNTER_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Service Encounter Handler Baseline",
            "EntityType: service_encounter",
            "Operation: create",
            "parse PayloadJson as CreateServiceEncounterRequest",
            "validate service is available for the visit brigade through BrigadeServices",
            "conflict duplicate VisitId plus ServiceId values inside the same pending batch",
            "reserved only after successful ServiceEncounter construction",
            "processor must process service_encounter create events before vital_signs create events",
            "service_encounter update is not implemented in P3-16",
            "service_encounter complete/close is not implemented in P3-16",
            "processor must not create forms, documents, referrals, or medication deliveries in P3-16",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor service encounter handler baseline");
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