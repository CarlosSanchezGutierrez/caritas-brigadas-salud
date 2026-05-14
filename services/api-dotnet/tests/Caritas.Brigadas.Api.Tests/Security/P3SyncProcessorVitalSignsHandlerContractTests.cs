using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorVitalSignsHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesVitalSignsCreateOnly()
    {
        var source =
            File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "VitalSignsSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "await _vitalSignsSyncEventHandler.HandleAsync(",
            "VitalSignsSyncEventHandler",
            "SyncEntityType.VitalSigns",
            "return 3;",
            "return 4;",
            "out CreateVitalSignsRecordRequest? request",
            "var vitalSignsRecord = new VitalSignsRecord(",
            "_dbContext.VitalSignsRecords.Add(vitalSignsRecord)",
            "syncEvent.Accept(",
            "vitalSignsRecord.Id",
            "vital_signs_operation_not_implemented",
            "vital_signs_patient_not_found",
            "vital_signs_visit_not_found",
            "vital_signs_encounter_not_found",
            "vital_signs_measured_by_user_not_found",
            "vital_signs_id_already_exists",
            "vital_signs_duplicate_in_pending_batch",
            "acceptedVitalSignsIdsInBatch"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor vital signs handler");
    }

    [Fact]
    public void SyncBatchProcessor_DoesNotContainDirectVitalSignsLogicAfterExtraction()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly VitalSignsSyncEventHandler _vitalSignsSyncEventHandler;",
            "_vitalSignsSyncEventHandler = new VitalSignsSyncEventHandler(dbContext, PayloadJsonOptions);",
            "    private async Task await _vitalSignsSyncEventHandler.HandleAsync(",
            "await _vitalSignsSyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor vital signs wrapper");

        var forbiddenTokens = new[]
        {
            "out CreateVitalSignsRecordRequest? request",
            "var vitalSignsRecord = new VitalSignsRecord(",
            "_dbContext.VitalSignsRecords.Add(vitalSignsRecord)",
            "vital_signs_operation_not_implemented",
            "vital_signs_patient_not_found",
            "vital_signs_visit_not_found",
            "vital_signs_encounter_not_found",
            "vital_signs_measured_by_user_not_found",
            "vital_signs_id_already_exists",
            "vital_signs_duplicate_in_pending_batch"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
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
