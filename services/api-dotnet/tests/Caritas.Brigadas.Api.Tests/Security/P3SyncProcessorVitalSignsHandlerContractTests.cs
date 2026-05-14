using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorVitalSignsHandlerContractTests
{
    [Fact]
    public void CreateVitalSignsRecordRequest_DefinesCanonicalClinicalUnits()
    {
        var source = File.ReadAllText(GetContractPath("VitalSigns", "CreateVitalSignsRecordRequest.cs"));

        var requiredTokens = new[]
        {
            "CreateVitalSignsRecordRequest",
            "PatientId",
            "VisitId",
            "EncounterId",
            "MeasuredByUserId",
            "MeasuredAt",
            "SystolicBloodPressureMmHg",
            "DiastolicBloodPressureMmHg",
            "HeartRateBpm",
            "RespiratoryRatePerMinute",
            "TemperatureCelsius",
            "OxygenSaturationPercent",
            "WeightKg",
            "HeightCm",
            "GlucoseMgDl",
            "DeviceId"
        };

        AssertRequiredTokens(source, requiredTokens, "CreateVitalSignsRecordRequest");
    }

    [Fact]
    public void SyncBatchProcessor_HandlesVitalSignsCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "HandleVitalSignsEventAsync",
            "syncEvent.EntityType == SyncEntityType.VitalSigns",
            "syncEvent.Operation != SyncOperation.Create",
            "vital_signs_operation_not_implemented",
            "JsonSerializer.Deserialize<CreateVitalSignsRecordRequest>",
            "new VitalSignsRecord(",
            "_dbContext.VitalSignsRecords.Add(vitalSignsRecord)",
            "syncEvent.Accept(",
            "vitalSignsRecord.Id",
            "vital_signs_patient_not_found",
            "vital_signs_visit_not_found",
            "vital_signs_encounter_not_found",
            "vital_signs_measured_by_user_not_found",
            "vital_signs_id_already_exists",
            "acceptedVitalSignsIdsInBatch",
            "acceptedVitalSignsIdsInBatch.Contains(vitalSignsRecordId)",
            "acceptedVitalSignsIdsInBatch.Add(vitalSignsRecordId)",
            "return 2;",
            "return 3;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor vital signs handler");

        var forbiddenTokens = new[]
        {
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void VitalSignsHandlerBaseline_DefinesVitalSignsOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_VITAL_SIGNS_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Vital Signs Handler Baseline",
            "EntityType: vital_signs",
            "Operation: create",
            "parse PayloadJson as CreateVitalSignsRecordRequest",
            "validate VisitId belongs to the same OrganizationId, PatientId, and parent SyncBatch.BrigadeId",
            "use canonical TemperatureCelsius",
            "processor must process patient_visit create events before vital_signs create events",
            "vital signs must remain historical records, not overwritten fields on Patient",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor vital signs handler baseline");
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

    private static string GetContractPath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Contracts" }
                .Concat(segments)
                .ToArray());
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