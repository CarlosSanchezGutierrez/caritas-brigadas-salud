using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfModelIntegrityContractTests
{
    [Fact]
    public void EntityDerivedTypes_HaveSingleIdPrimaryKeyAndClientGeneratedGuid()
    {
        var model = CreateModel();

        var failures = new List<string>();

        foreach (var entityType in GetEntityDerivedTypes(model))
        {
            var primaryKey = entityType.FindPrimaryKey();

            if (primaryKey is null)
            {
                failures.Add($"{entityType.ClrType.Name} does not define a primary key.");
                continue;
            }

            var keyProperties = primaryKey.Properties.Select(property => property.Name).ToArray();

            if (!keyProperties.SequenceEqual(new[] { nameof(Entity.Id) }, StringComparer.Ordinal))
            {
                failures.Add($"{entityType.ClrType.Name} primary key must be exactly Id.");
                continue;
            }

            var idProperty = entityType.FindProperty(nameof(Entity.Id));

            if (idProperty is null)
            {
                failures.Add($"{entityType.ClrType.Name} does not map Id.");
                continue;
            }

            if (idProperty.ClrType != typeof(Guid))
            {
                failures.Add($"{entityType.ClrType.Name}.Id must be Guid.");
            }

            if (idProperty.ValueGenerated != ValueGenerated.Never)
            {
                failures.Add($"{entityType.ClrType.Name}.Id must be client generated with ValueGeneratedNever.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void AuditableEntities_HaveCreatedAtAndSoftDeleteContract()
    {
        var model = CreateModel();

        var failures = new List<string>();

        foreach (var entityType in GetAuditableEntityTypes(model))
        {
            AssertRequiredProperty(entityType, nameof(AuditableEntity.CreatedAt), failures);
            AssertRequiredProperty(entityType, nameof(SoftDeletableEntity.IsDeleted), failures);

            if (!HasIndex(entityType, nameof(SoftDeletableEntity.IsDeleted)))
            {
                failures.Add($"{entityType.ClrType.Name} must have an IsDeleted index.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void CriticalUniqueIndexes_AreConfigured()
    {
        var model = CreateModel();

        var expectedUniqueIndexes = new[]
        {
            IndexExpectation.Unique<Permission>(nameof(Permission.Code)),
            IndexExpectation.Unique<Role>(nameof(Role.OrganizationId), nameof(Role.Code)),
            IndexExpectation.Unique<RolePermission>(nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId)),
            IndexExpectation.Unique<Service>(nameof(Service.OrganizationId), nameof(Service.Code)),
            IndexExpectation.Unique<BrigadeService>(nameof(BrigadeService.BrigadeId), nameof(BrigadeService.ServiceId)),
            IndexExpectation.Unique<Patient>(nameof(Patient.OrganizationId), nameof(Patient.PatientFolio)),
            IndexExpectation.Unique<PatientVisit>(nameof(PatientVisit.OrganizationId), nameof(PatientVisit.VisitFolio)),
            IndexExpectation.Unique<ServiceEncounter>(nameof(ServiceEncounter.OrganizationId), nameof(ServiceEncounter.EncounterFolio)),
            IndexExpectation.Unique<MedicalReferral>(nameof(MedicalReferral.OrganizationId), nameof(MedicalReferral.ReferralFolio)),
            IndexExpectation.Unique<FormTemplate>(
                nameof(FormTemplate.OrganizationId),
                nameof(FormTemplate.ServiceId),
                nameof(FormTemplate.FormCode),
                nameof(FormTemplate.Version)),
            IndexExpectation.Unique<SyncEvent>(nameof(SyncEvent.SyncBatchId), nameof(SyncEvent.LocalEventId))
        };

        AssertIndexes(model, expectedUniqueIndexes);
    }

    [Fact]
    public void CriticalLookupIndexes_AreConfigured()
    {
        var model = CreateModel();

        var expectedLookupIndexes = new[]
        {
            IndexExpectation.NonUnique<Organization>(nameof(Organization.Name)),
            IndexExpectation.NonUnique<User>(nameof(User.OrganizationId), nameof(User.Email)),
            IndexExpectation.NonUnique<User>(nameof(User.OrganizationId), nameof(User.Username)),
            IndexExpectation.NonUnique<UserRole>(
                nameof(UserRole.OrganizationId),
                nameof(UserRole.UserId),
                nameof(UserRole.RoleId)),
            IndexExpectation.NonUnique<Community>(
                nameof(Community.OrganizationId),
                nameof(Community.Municipality),
                nameof(Community.Colony)),
            IndexExpectation.NonUnique<MobileUnit>(nameof(MobileUnit.OrganizationId), nameof(MobileUnit.Name)),
            IndexExpectation.NonUnique<Brigade>(nameof(Brigade.OrganizationId), nameof(Brigade.ScheduledDate)),
            IndexExpectation.NonUnique<Patient>(nameof(Patient.OrganizationId), nameof(Patient.FullNameNormalized)),
            IndexExpectation.NonUnique<PatientVisit>(nameof(PatientVisit.BrigadeId), nameof(PatientVisit.PatientId)),
            IndexExpectation.NonUnique<ServiceEncounter>(nameof(ServiceEncounter.VisitId), nameof(ServiceEncounter.ServiceId)),
            IndexExpectation.NonUnique<FormResponse>(nameof(FormResponse.OrganizationId), nameof(FormResponse.EncounterId)),
            IndexExpectation.NonUnique<DocumentTemplate>(
                nameof(DocumentTemplate.OrganizationId),
                nameof(DocumentTemplate.DocumentType),
                nameof(DocumentTemplate.Version)),
            IndexExpectation.NonUnique<DocumentSignature>(nameof(DocumentSignature.OrganizationId), nameof(DocumentSignature.DocumentTemplateId)),
            IndexExpectation.NonUnique<MediaRelease>(nameof(MediaRelease.OrganizationId), nameof(MediaRelease.PatientId)),
            IndexExpectation.NonUnique<SyncBatch>(
                nameof(SyncBatch.OrganizationId),
                nameof(SyncBatch.DeviceId),
                nameof(SyncBatch.StartedAt)),
            IndexExpectation.NonUnique<AuditEvent>(nameof(AuditEvent.OrganizationId), nameof(AuditEvent.CreatedAt)),
            IndexExpectation.NonUnique<AuditEvent>(nameof(AuditEvent.EntityType), nameof(AuditEvent.EntityId)),
            IndexExpectation.NonUnique<ExportJob>(nameof(ExportJob.OrganizationId), nameof(ExportJob.RequestedAt)),
            IndexExpectation.NonUnique<AiRequestLog>(nameof(AiRequestLog.OrganizationId), nameof(AiRequestLog.RequestedAt)),
            IndexExpectation.NonUnique<CryptoIntegrityRecord>(
                nameof(CryptoIntegrityRecord.OrganizationId),
                nameof(CryptoIntegrityRecord.EntityType),
                nameof(CryptoIntegrityRecord.EntityId))
        };

        AssertIndexes(model, expectedLookupIndexes);
    }

    [Fact]
    public void CriticalStringFields_HaveExpectedMaxLengthsAndRequiredFlags()
    {
        var model = CreateModel();

        var expectations = new[]
        {
            PropertyExpectation.Required<Organization>(nameof(Organization.Name), 200),
            PropertyExpectation.Optional<Organization>(nameof(Organization.LegalName), 250),
            PropertyExpectation.Optional<Organization>(nameof(Organization.Rfc), 20),
            PropertyExpectation.Optional<Organization>(nameof(Organization.Email), 200),
            PropertyExpectation.Required<Organization>(nameof(Organization.Status), 50),

            PropertyExpectation.Required<User>(nameof(User.FullName), 200),
            PropertyExpectation.Optional<User>(nameof(User.Email), 200),
            PropertyExpectation.Optional<User>(nameof(User.Username), 100),
            PropertyExpectation.Required<User>(nameof(User.Status), 50),

            PropertyExpectation.Required<Role>(nameof(Role.Code), 100),
            PropertyExpectation.Required<Role>(nameof(Role.Name), 150),
            PropertyExpectation.Required<Role>(nameof(Role.Status), 50),

            PropertyExpectation.Required<Permission>(nameof(Permission.Code), 150),
            PropertyExpectation.Required<Permission>(nameof(Permission.Name), 200),
            PropertyExpectation.Required<Permission>(nameof(Permission.Module), 100),
            PropertyExpectation.Required<Permission>(nameof(Permission.Action), 100),
            PropertyExpectation.Required<Permission>(nameof(Permission.SensitivityLevel), 50),

            PropertyExpectation.Required<Service>(nameof(Service.Code), 100),
            PropertyExpectation.Required<Service>(nameof(Service.Name), 200),
            PropertyExpectation.Required<Service>(nameof(Service.Category), 100),
            PropertyExpectation.Required<Service>(nameof(Service.Status), 50),

            PropertyExpectation.Required<Patient>(nameof(Patient.PatientFolio), 50),
            PropertyExpectation.Optional<Patient>(nameof(Patient.FirstName), 150),
            PropertyExpectation.Optional<Patient>(nameof(Patient.PaternalLastName), 150),
            PropertyExpectation.Optional<Patient>(nameof(Patient.MaternalLastName), 150),
            PropertyExpectation.Optional<Patient>(nameof(Patient.FullNameNormalized), 400),
            PropertyExpectation.Optional<Patient>(nameof(Patient.Curp), 30),
            PropertyExpectation.Optional<Patient>(nameof(Patient.Phone), 50),
            PropertyExpectation.Required<Patient>(nameof(Patient.Status), 50)
        };

        var failures = new List<string>();

        foreach (var expectation in expectations)
        {
            var entityType = FindEntityType(model, expectation.EntityClrType);
            var property = entityType.FindProperty(expectation.PropertyName);

            if (property is null)
            {
                failures.Add($"{expectation.EntityClrType.Name}.{expectation.PropertyName} is not mapped.");
                continue;
            }

            if (property.GetMaxLength() != expectation.MaxLength)
            {
                failures.Add(
                    $"{expectation.EntityClrType.Name}.{expectation.PropertyName} max length expected {expectation.MaxLength}, actual {property.GetMaxLength()?.ToString() ?? "null"}.");
            }

            if (property.IsNullable == expectation.IsRequired)
            {
                var expected = expectation.IsRequired ? "required" : "optional";
                failures.Add($"{expectation.EntityClrType.Name}.{expectation.PropertyName} must be {expected}.");
            }
        }

        AssertNoFailures(failures);
    }

    private static IModel CreateModel()
    {
        var options = new DbContextOptionsBuilder<CaritasDbContext>()
            .UseSqlServer("Server=localhost;Database=Caritas_ModelOnly;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var dbContext = new CaritasDbContext(options);

        return dbContext.Model;
    }

    private static IEnumerable<IEntityType> GetEntityDerivedTypes(IModel model)
    {
        return model
            .GetEntityTypes()
            .Where(entityType => typeof(Entity).IsAssignableFrom(entityType.ClrType))
            .OrderBy(entityType => entityType.ClrType.Name, StringComparer.Ordinal);
    }

    private static IEnumerable<IEntityType> GetAuditableEntityTypes(IModel model)
    {
        return model
            .GetEntityTypes()
            .Where(entityType => typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            .OrderBy(entityType => entityType.ClrType.Name, StringComparer.Ordinal);
    }

    private static void AssertRequiredProperty(
        IEntityType entityType,
        string propertyName,
        ICollection<string> failures)
    {
        var property = entityType.FindProperty(propertyName);

        if (property is null)
        {
            failures.Add($"{entityType.ClrType.Name}.{propertyName} is not mapped.");
            return;
        }

        if (property.IsNullable)
        {
            failures.Add($"{entityType.ClrType.Name}.{propertyName} must be required.");
        }
    }

    private static bool HasIndex(IEntityType entityType, params string[] propertyNames)
    {
        return entityType
            .GetIndexes()
            .Any(index => IndexPropertiesMatch(index, propertyNames));
    }

    private static void AssertIndexes(
        IModel model,
        IReadOnlyCollection<IndexExpectation> expectations)
    {
        var failures = new List<string>();

        foreach (var expectation in expectations)
        {
            var entityType = FindEntityType(model, expectation.EntityClrType);

            var index = entityType
                .GetIndexes()
                .SingleOrDefault(candidate => IndexPropertiesMatch(candidate, expectation.PropertyNames));

            if (index is null)
            {
                failures.Add($"{expectation.EntityClrType.Name} missing index ({string.Join(", ", expectation.PropertyNames)}).");
                continue;
            }

            if (index.IsUnique != expectation.IsUnique)
            {
                var expected = expectation.IsUnique ? "unique" : "non-unique";
                failures.Add($"{expectation.EntityClrType.Name} index ({string.Join(", ", expectation.PropertyNames)}) must be {expected}.");
            }
        }

        AssertNoFailures(failures);
    }

    private static IEntityType FindEntityType(IModel model, Type clrType)
    {
        return model.FindEntityType(clrType)
            ?? throw new InvalidOperationException($"{clrType.Name} is not mapped in CaritasDbContext.");
    }

    private static bool IndexPropertiesMatch(
        IIndex index,
        IReadOnlyCollection<string> propertyNames)
    {
        return index.Properties
            .Select(property => property.Name)
            .SequenceEqual(propertyNames, StringComparer.Ordinal);
    }

    private static void AssertNoFailures(IReadOnlyCollection<string> failures)
    {
        Assert.True(
            failures.Count == 0,
            string.Join(Environment.NewLine, failures));
    }

    private sealed record IndexExpectation(
        Type EntityClrType,
        IReadOnlyCollection<string> PropertyNames,
        bool IsUnique)
    {
        public static IndexExpectation Unique<TEntity>(params string[] propertyNames)
        {
            return new IndexExpectation(typeof(TEntity), propertyNames, true);
        }

        public static IndexExpectation NonUnique<TEntity>(params string[] propertyNames)
        {
            return new IndexExpectation(typeof(TEntity), propertyNames, false);
        }
    }

    private sealed record PropertyExpectation(
        Type EntityClrType,
        string PropertyName,
        int MaxLength,
        bool IsRequired)
    {
        public static PropertyExpectation Required<TEntity>(
            string propertyName,
            int maxLength)
        {
            return new PropertyExpectation(typeof(TEntity), propertyName, maxLength, true);
        }

        public static PropertyExpectation Optional<TEntity>(
            string propertyName,
            int maxLength)
        {
            return new PropertyExpectation(typeof(TEntity), propertyName, maxLength, false);
        }
    }
}