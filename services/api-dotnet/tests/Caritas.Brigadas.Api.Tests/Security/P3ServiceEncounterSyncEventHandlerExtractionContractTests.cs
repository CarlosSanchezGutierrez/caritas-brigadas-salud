using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ServiceEncounterSyncEventHandlerExtractionContractTests
{
    [Fact]
    public void ServiceEncounterSyncEventHandler_OwnsServiceEncounterCreateBehavior()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "ServiceEncounterSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "internal sealed class ServiceEncounterSyncEventHandler",
            "public async Task HandleAsync",
            "SyncPayloadReader.TryReadObject",
            "out CreateServiceEncounterRequest? request",
            "var encounter = new ServiceEncounter(",
            "_dbContext.ServiceEncounters.Add(encounter)",
            "syncEvent.Accept(",
            "encounter.Id",
            "service_encounter_operation_not_implemented",
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
            "reserved only after successful ServiceEncounter construction and reserved atomically",
            "encounterFolioReserved",
            "encounterVisitServiceKeyReserved",
            "acceptedEncounterFoliosInBatch.Remove(normalizedEncounterFolio)"
        };

        AssertRequiredTokens(source, requiredTokens, "ServiceEncounterSyncEventHandler");
    }

    [Fact]
    public void SyncBatchProcessor_DelegatesServiceEncounterCreateToServiceEncounterSyncEventHandler()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly ServiceEncounterSyncEventHandler _serviceEncounterSyncEventHandler;",
            "_serviceEncounterSyncEventHandler = new ServiceEncounterSyncEventHandler(dbContext, PayloadJsonOptions);",
            "    private async Task HandleServiceEncounterEventAsync",
            "await _serviceEncounterSyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor service encounter handler extraction");

        var forbiddenTokens = new[]
        {
            "out CreateServiceEncounterRequest? request",
            "var encounter = new ServiceEncounter(",
            "_dbContext.ServiceEncounters.Add(encounter)",
            "service_encounter_operation_not_implemented",
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
            "GenerateSyncEncounterFolio("
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ServiceEncounterHandlerExtractionBaseline_DefinesThirdHandlerExtraction()
    {
        var source = File.ReadAllText(GetDocPath("P3_SERVICE_ENCOUNTER_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Service Encounter Sync Event Handler Extraction Baseline",
            "ServiceEncounterSyncEventHandler must own service_encounter/create payload parsing",
            "SyncBatchProcessor must not directly construct ServiceEncounter",
            "SyncBatchProcessor must not directly parse CreateServiceEncounterRequest",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 service encounter sync event handler extraction baseline");
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
