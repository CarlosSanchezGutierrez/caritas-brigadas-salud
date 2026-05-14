using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ClinicalRecordReadModelContractTests
{
    [Fact]
    public void PatientClinicalRecordDto_IncludesPatientVisitsEncountersVitalSignsAndSummary()
    {
        var source = File.ReadAllText(GetContractPath("Patients", "PatientClinicalRecordDto.cs"));

        var requiredTokens = new[]
        {
            "PatientClinicalRecordDto",
            "PatientSummaryDto Patient",
            "IReadOnlyCollection<PatientClinicalRecordVisitDto> Visits",
            "IReadOnlyCollection<PatientClinicalRecordEncounterDto> Encounters",
            "IReadOnlyCollection<PatientClinicalRecordVitalSignsDto> VitalSigns",
            "IReadOnlyCollection<PatientClinicalRecordFormResponseDto> FormResponses",
            "IReadOnlyCollection<PatientClinicalRecordConsentDocumentDto> ConsentDocuments",
            "IReadOnlyCollection<PatientClinicalRecordMedicalReferralDto> MedicalReferrals",
            "IReadOnlyCollection<PatientClinicalRecordMedicationDeliveryDto> MedicationDeliveries",
            "PatientClinicalRecordSummaryDto Summary",
            "FormResponseCount",
            "ConsentDocumentCount",
            "MedicalReferralCount",
            "MedicationDeliveryCount",
            "SystolicBloodPressureMmHg",
            "DiastolicBloodPressureMmHg",
            "HeartRateBpm",
            "RespiratoryRatePerMinute",
            "TemperatureCelsius",
            "OxygenSaturationPercent",
            "WeightKg",
            "HeightCm",
            "GlucoseMgDl"
        };

        AssertRequiredTokens(source, requiredTokens, "Patient clinical record DTO");
    }

    [Fact]
    public void PatientReadRepository_ClinicalRecordQueryIsTenantScopedAndIncludesVitalSigns()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Patients", "PatientReadRepository.cs"));

        var requiredTokens = new[]
        {
            "GetClinicalRecordAsync",
            "Guid organizationId",
            "Guid patientId",
            "entity.OrganizationId == organizationId",
            "entity.PatientId == patientId",
            "_dbContext.PatientVisits",
            "_dbContext.ServiceEncounters",
            "_dbContext.VitalSignsRecords",
            "_dbContext.FormResponses",
            "_dbContext.ConsentDocuments",
            "_dbContext.MedicalReferrals",
            "_dbContext.MedicationDeliveries",
            "PatientClinicalRecordDto",
            "PatientClinicalRecordSummaryDto",
            "ResponseHash",
            "HasSignature",
            "ReferralFolio",
            "MedicationName"
        };

        AssertRequiredTokens(source, requiredTokens, "PatientReadRepository clinical record query");
    }

    [Fact]
    public void PatientsController_ExposesTenantScopedClinicalRecordEndpoint()
    {
        var source = File.ReadAllText(GetControllerPath("PatientsController.cs"));

        var requiredTokens = new[]
        {
            "GetClinicalRecordAsync",
            "api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}/clinical-record",
            "Authorize(Policy = PermissionCodes.PatientsRead)",
            "repository.GetClinicalRecordAsync(",
            "organizationId",
            "patientId",
            "PatientClinicalRecordDto"
        };

        AssertRequiredTokens(source, requiredTokens, "PatientsController clinical record endpoint");
    }

    [Fact]
    public void PatientReadRepositoryInterface_DefinesClinicalRecordReadContract()
    {
        var source = File.ReadAllText(GetApplicationPath("Patients", "IPatientReadRepository.cs"));

        var requiredTokens = new[]
        {
            "Task<PatientClinicalRecordDto?> GetClinicalRecordAsync",
            "Guid organizationId",
            "Guid patientId"
        };

        AssertRequiredTokens(source, requiredTokens, "IPatientReadRepository clinical record contract");
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
            $"{label} contract is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetApplicationPath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Application" }
                .Concat(segments)
                .ToArray());
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

    private static string GetControllerPath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Api", "Controllers" }
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
