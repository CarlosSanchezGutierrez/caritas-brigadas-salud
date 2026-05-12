using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfFormsDocumentsSyncForeignKeyContractTests
{
    [Fact]
    public void FormsDocumentsSyncForeignKeys_AreConfiguredWithNoActionDeleteBehavior()
    {
        var model = CreateModel();

        var expectations = new[]
        {
            ForeignKeyExpectation.Required<FormTemplate, Organization>(nameof(FormTemplate.OrganizationId)),
            ForeignKeyExpectation.Required<FormTemplate, Service>(nameof(FormTemplate.ServiceId)),
            ForeignKeyExpectation.Required<FormResponse, Organization>(nameof(FormResponse.OrganizationId)),
            ForeignKeyExpectation.Required<FormResponse, FormTemplate>(nameof(FormResponse.FormTemplateId)),
            ForeignKeyExpectation.Required<FormResponse, ServiceEncounter>(nameof(FormResponse.EncounterId)),

            ForeignKeyExpectation.Required<DocumentTemplate, Organization>(nameof(DocumentTemplate.OrganizationId)),
            ForeignKeyExpectation.Optional<DocumentTemplate, Service>(nameof(DocumentTemplate.AppliesToServiceId)),
            ForeignKeyExpectation.Required<DocumentSignature, Organization>(nameof(DocumentSignature.OrganizationId)),
            ForeignKeyExpectation.Required<DocumentSignature, DocumentTemplate>(nameof(DocumentSignature.DocumentTemplateId)),
            ForeignKeyExpectation.Optional<DocumentSignature, Patient>(nameof(DocumentSignature.PatientId)),
            ForeignKeyExpectation.Optional<DocumentSignature, PatientVisit>(nameof(DocumentSignature.VisitId)),
            ForeignKeyExpectation.Optional<DocumentSignature, ServiceEncounter>(nameof(DocumentSignature.EncounterId)),
            ForeignKeyExpectation.Required<MediaRelease, Organization>(nameof(MediaRelease.OrganizationId)),
            ForeignKeyExpectation.Required<MediaRelease, Patient>(nameof(MediaRelease.PatientId)),
            ForeignKeyExpectation.Optional<MediaRelease, PatientVisit>(nameof(MediaRelease.VisitId)),

            ForeignKeyExpectation.Required<SyncBatch, Organization>(nameof(SyncBatch.OrganizationId)),
            ForeignKeyExpectation.Optional<SyncBatch, Brigade>(nameof(SyncBatch.BrigadeId)),
            ForeignKeyExpectation.Required<SyncEvent, Organization>(nameof(SyncEvent.OrganizationId)),
            ForeignKeyExpectation.Required<SyncEvent, SyncBatch>(nameof(SyncEvent.SyncBatchId))
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

            if (foreignKey.IsRequired != expectation.IsRequired)
            {
                var expected = expectation.IsRequired ? "required" : "optional";
                failures.Add($"{expectation.Describe()} must be {expected}.");
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
            "Forms/documents/sync foreign key contracts failed." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void FormsDocumentsSyncForeignKeyCount_MatchesP208Package()
    {
        var model = CreateModel();

        var foreignKeys = model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey =>
            {
                var schema = foreignKey.DeclaringEntityType.GetSchema();

                return schema is "forms" or "documents" or "sync";
            })
            .Select(DescribeForeignKey)
            .Order(StringComparer.Ordinal)
            .ToArray();

        var expected = new[]
        {
            "documents.document_signatures(DocumentTemplateId) -> documents.document_templates(Id) DeleteBehavior=NoAction",
            "documents.document_signatures(EncounterId) -> clinical.service_encounters(Id) DeleteBehavior=NoAction",
            "documents.document_signatures(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "documents.document_signatures(PatientId) -> clinical.patients(Id) DeleteBehavior=NoAction",
            "documents.document_signatures(VisitId) -> clinical.patient_visits(Id) DeleteBehavior=NoAction",
            "documents.document_templates(AppliesToServiceId) -> core.services(Id) DeleteBehavior=NoAction",
            "documents.document_templates(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "documents.media_releases(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "documents.media_releases(PatientId) -> clinical.patients(Id) DeleteBehavior=NoAction",
            "documents.media_releases(VisitId) -> clinical.patient_visits(Id) DeleteBehavior=NoAction",
            "forms.form_responses(EncounterId) -> clinical.service_encounters(Id) DeleteBehavior=NoAction",
            "forms.form_responses(FormTemplateId) -> forms.form_templates(Id) DeleteBehavior=NoAction",
            "forms.form_responses(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "forms.form_templates(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "forms.form_templates(ServiceId) -> core.services(Id) DeleteBehavior=NoAction",
            "sync.sync_batches(BrigadeId) -> brigades.brigades(Id) DeleteBehavior=NoAction",
            "sync.sync_batches(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "sync.sync_events(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "sync.sync_events(SyncBatchId) -> sync.sync_batches(Id) DeleteBehavior=NoAction"
        };

        Assert.Equal(expected, foreignKeys);
    }

    [Fact]
    public void SyncBatchDeviceId_RemainsDeferredAndIsNotConfiguredAsForeignKey()
    {
        var model = CreateModel();
        var syncBatch = FindEntityType(model, typeof(SyncBatch));

        var hasDeviceForeignKey = syncBatch
            .GetForeignKeys()
            .Any(foreignKey =>
                foreignKey.PrincipalEntityType.ClrType == typeof(Device) &&
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(
                    new[] { nameof(SyncBatch.DeviceId) },
                    StringComparer.Ordinal));

        Assert.False(
            hasDeviceForeignKey,
            "SyncBatch.DeviceId must remain deferred until the offline/revoked device policy is defined.");
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
        Type PrincipalClrType,
        bool IsRequired)
    {
        public static ForeignKeyExpectation Required<TDependent, TPrincipal>(
            string dependentPropertyName)
        {
            return new ForeignKeyExpectation(
                typeof(TDependent),
                dependentPropertyName,
                typeof(TPrincipal),
                true);
        }

        public static ForeignKeyExpectation Optional<TDependent, TPrincipal>(
            string dependentPropertyName)
        {
            return new ForeignKeyExpectation(
                typeof(TDependent),
                dependentPropertyName,
                typeof(TPrincipal),
                false);
        }

        public string Describe()
        {
            return $"{DependentClrType.Name}.{DependentPropertyName} -> {PrincipalClrType.Name}.Id";
        }
    }
}