using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfClinicalForeignKeyContractTests
{
    [Fact]
    public void ClinicalForeignKeys_AreConfiguredWithNoActionDeleteBehavior()
    {
        var model = CreateModel();

        var expectations = new[]
        {
            ForeignKeyExpectation.Required<Patient, Organization>(nameof(Patient.OrganizationId)),
            ForeignKeyExpectation.Required<PatientGuardian, Patient>(nameof(PatientGuardian.PatientId)),
            ForeignKeyExpectation.Required<PatientVisit, Organization>(nameof(PatientVisit.OrganizationId)),
            ForeignKeyExpectation.Required<PatientVisit, Patient>(nameof(PatientVisit.PatientId)),
            ForeignKeyExpectation.Required<PatientVisit, Brigade>(nameof(PatientVisit.BrigadeId)),
            ForeignKeyExpectation.Required<ServiceEncounter, Organization>(nameof(ServiceEncounter.OrganizationId)),
            ForeignKeyExpectation.Required<ServiceEncounter, Patient>(nameof(ServiceEncounter.PatientId)),
            ForeignKeyExpectation.Required<ServiceEncounter, PatientVisit>(nameof(ServiceEncounter.VisitId)),
            ForeignKeyExpectation.Required<ServiceEncounter, Brigade>(nameof(ServiceEncounter.BrigadeId)),
            ForeignKeyExpectation.Required<ServiceEncounter, Service>(nameof(ServiceEncounter.ServiceId)),
            ForeignKeyExpectation.Required<MedicalReferral, Organization>(nameof(MedicalReferral.OrganizationId)),
            ForeignKeyExpectation.Required<MedicalReferral, Patient>(nameof(MedicalReferral.PatientId)),
            ForeignKeyExpectation.Required<MedicalReferral, ServiceEncounter>(nameof(MedicalReferral.EncounterId)),
            ForeignKeyExpectation.Required<MedicationDelivery, Organization>(nameof(MedicationDelivery.OrganizationId)),
            ForeignKeyExpectation.Required<MedicationDelivery, Patient>(nameof(MedicationDelivery.PatientId)),
            ForeignKeyExpectation.Required<MedicationDelivery, ServiceEncounter>(nameof(MedicationDelivery.EncounterId))
        };

        var failures = new List<string>();

        foreach (var expectation in expectations)
        {
            var dependentEntityType = FindEntityType(model, expectation.DependentClrType);
            var principalEntityType = FindEntityType(model, expectation.PrincipalClrType);

            var foreignKey = dependentEntityType
                .GetForeignKeys()
                .SingleOrDefault(candidate =>
                    candidate.PrincipalEntityType == principalEntityType &&
                    candidate.Properties.Select(property => property.Name).SequenceEqual(
                        new[] { expectation.DependentPropertyName },
                        StringComparer.Ordinal));

            if (foreignKey is null)
            {
                failures.Add($"{expectation.Describe()} is not configured.");
                continue;
            }

            if (!foreignKey.IsRequired)
            {
                failures.Add($"{expectation.Describe()} must be required.");
            }

            if (foreignKey.DeleteBehavior != DeleteBehavior.NoAction)
            {
                failures.Add($"{expectation.Describe()} must use DeleteBehavior.NoAction. Actual: {foreignKey.DeleteBehavior}.");
            }

            if (foreignKey.PrincipalKey.Properties.Select(property => property.Name).SingleOrDefault() != "Id")
            {
                failures.Add($"{expectation.Describe()} must target principal Id.");
            }
        }

        Assert.True(
            failures.Count == 0,
            "Clinical foreign key contracts failed." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ClinicalForeignKeyCount_MatchesP207Package()
    {
        var model = CreateModel();

        var clinicalForeignKeys = model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey =>
                foreignKey.DeclaringEntityType.GetSchema() == "clinical" ||
                foreignKey.PrincipalEntityType.GetSchema() == "clinical")
            .Select(DescribeForeignKey)
            .Order(StringComparer.Ordinal)
            .ToArray();

        var expected = new[]
        {
            "clinical.medical_referrals(EncounterId) -> clinical.service_encounters(Id) DeleteBehavior=NoAction",
            "clinical.medical_referrals(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "clinical.medical_referrals(PatientId) -> clinical.patients(Id) DeleteBehavior=NoAction",
            "clinical.medication_deliveries(EncounterId) -> clinical.service_encounters(Id) DeleteBehavior=NoAction",
            "clinical.medication_deliveries(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "clinical.medication_deliveries(PatientId) -> clinical.patients(Id) DeleteBehavior=NoAction",
            "clinical.patient_guardians(PatientId) -> clinical.patients(Id) DeleteBehavior=NoAction",
            "clinical.patient_visits(BrigadeId) -> brigades.brigades(Id) DeleteBehavior=NoAction",
            "clinical.patient_visits(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "clinical.patient_visits(PatientId) -> clinical.patients(Id) DeleteBehavior=NoAction",
            "clinical.patients(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "clinical.service_encounters(BrigadeId) -> brigades.brigades(Id) DeleteBehavior=NoAction",
            "clinical.service_encounters(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "clinical.service_encounters(PatientId) -> clinical.patients(Id) DeleteBehavior=NoAction",
            "clinical.service_encounters(ServiceId) -> core.services(Id) DeleteBehavior=NoAction",
            "clinical.service_encounters(VisitId) -> clinical.patient_visits(Id) DeleteBehavior=NoAction"
        };

        Assert.Equal(expected, clinicalForeignKeys);
    }

    private static IModel CreateModel()
    {
        var options = new DbContextOptionsBuilder<CaritasDbContext>()
            .UseSqlServer("Server=localhost;Database=Caritas_ModelOnly;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var dbContext = new CaritasDbContext(options);

        return dbContext.Model;
    }

    private static IEntityType FindEntityType(IModel model, Type clrType)
    {
        return model.FindEntityType(clrType)
            ?? throw new InvalidOperationException($"{clrType.Name} is not mapped in CaritasDbContext.");
    }

    private static string DescribeForeignKey(IForeignKey foreignKey)
    {
        var dependent = $"{foreignKey.DeclaringEntityType.GetSchema()}.{foreignKey.DeclaringEntityType.GetTableName()}";
        var principal = $"{foreignKey.PrincipalEntityType.GetSchema()}.{foreignKey.PrincipalEntityType.GetTableName()}";
        var properties = string.Join(", ", foreignKey.Properties.Select(property => property.Name));
        var principalKey = string.Join(", ", foreignKey.PrincipalKey.Properties.Select(property => property.Name));

        return $"{dependent}({properties}) -> {principal}({principalKey}) DeleteBehavior={foreignKey.DeleteBehavior}";
    }

    private sealed record ForeignKeyExpectation(
        Type DependentClrType,
        string DependentPropertyName,
        Type PrincipalClrType)
    {
        public static ForeignKeyExpectation Required<TDependent, TPrincipal>(
            string dependentPropertyName)
        {
            return new ForeignKeyExpectation(
                typeof(TDependent),
                dependentPropertyName,
                typeof(TPrincipal));
        }

        public string Describe()
        {
            return $"{DependentClrType.Name}.{DependentPropertyName} -> {PrincipalClrType.Name}.Id";
        }
    }
}