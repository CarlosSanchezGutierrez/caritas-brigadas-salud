using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfDeleteBehaviorContractTests
{
    [Fact]
    public void EfModel_DoesNotUseCascadeDeleteForProtectedAggregateRoots()
    {
        var model = CreateModel();

        var protectedPrincipalTypes = new[]
        {
            typeof(Organization),
            typeof(User),
            typeof(Role),
            typeof(Permission),
            typeof(Service),
            typeof(Brigade),
            typeof(Patient),
            typeof(PatientVisit),
            typeof(ServiceEncounter),
            typeof(FormTemplate),
            typeof(DocumentTemplate),
            typeof(SyncBatch)
        };

        var failures = model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey => foreignKey.PrincipalEntityType.ClrType is not null)
            .Where(foreignKey => protectedPrincipalTypes.Contains(foreignKey.PrincipalEntityType.ClrType))
            .Where(foreignKey => foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
            .Select(DescribeForeignKey)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "Protected aggregate roots must not cascade-delete dependents. Use Restrict, NoAction, ClientNoAction, or explicit soft-delete workflows instead." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void EfModel_DoesNotUseCascadeDeleteForClinicalDocumentsAuditOperationsOrSyncSchemas()
    {
        var model = CreateModel();

        var protectedSchemas = new[]
        {
            "clinical",
            "documents",
            "forms",
            "audit",
            "operations",
            "sync"
        };

        var failures = model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey =>
            {
                var dependentSchema = foreignKey.DeclaringEntityType.GetSchema();
                var principalSchema = foreignKey.PrincipalEntityType.GetSchema();

                return protectedSchemas.Contains(dependentSchema, StringComparer.OrdinalIgnoreCase) ||
                    protectedSchemas.Contains(principalSchema, StringComparer.OrdinalIgnoreCase);
            })
            .Where(foreignKey => foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
            .Select(DescribeForeignKey)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "Clinical, document, forms, audit, operations, and sync schemas must not use cascade delete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void EfModel_CurrentlyHasNoForeignKeysUntilP2RelationshipPackagesAreIntroduced()
    {
        var model = CreateModel();

        var foreignKeys = model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Select(DescribeForeignKey)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            foreignKeys.Length == 0,
            "The current P2 baseline should not introduce implicit relationship drift before explicit FK packages are reviewed." +
            Environment.NewLine +
            string.Join(Environment.NewLine, foreignKeys));
    }

    private static IModel CreateModel()
    {
        var options = new DbContextOptionsBuilder<CaritasDbContext>()
            .UseSqlServer("Server=localhost;Database=Caritas_ModelOnly;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var dbContext = new CaritasDbContext(options);

        return dbContext.Model;
    }

    private static string DescribeForeignKey(IForeignKey foreignKey)
    {
        var dependent = $"{foreignKey.DeclaringEntityType.GetSchema()}.{foreignKey.DeclaringEntityType.GetTableName()}";
        var principal = $"{foreignKey.PrincipalEntityType.GetSchema()}.{foreignKey.PrincipalEntityType.GetTableName()}";
        var properties = string.Join(", ", foreignKey.Properties.Select(property => property.Name));
        var principalKey = string.Join(", ", foreignKey.PrincipalKey.Properties.Select(property => property.Name));

        return $"{dependent}({properties}) -> {principal}({principalKey}) DeleteBehavior={foreignKey.DeleteBehavior}";
    }
}