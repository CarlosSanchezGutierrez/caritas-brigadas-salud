using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfBrigadesForeignKeyContractTests
{
    [Fact]
    public void BrigadesForeignKeys_AreConfiguredWithNoActionDeleteBehavior()
    {
        var model = CreateModel();

        var expectations = new[]
        {
            ForeignKeyExpectation.Required<Community, Organization>(nameof(Community.OrganizationId)),
            ForeignKeyExpectation.Required<MobileUnit, Organization>(nameof(MobileUnit.OrganizationId)),
            ForeignKeyExpectation.Required<Brigade, Organization>(nameof(Brigade.OrganizationId)),
            ForeignKeyExpectation.Optional<Brigade, Community>(nameof(Brigade.CommunityId)),
            ForeignKeyExpectation.Optional<Brigade, MobileUnit>(nameof(Brigade.MobileUnitId)),
            ForeignKeyExpectation.Required<BrigadeService, Brigade>(nameof(BrigadeService.BrigadeId)),
            ForeignKeyExpectation.Required<BrigadeService, Service>(nameof(BrigadeService.ServiceId))
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
            "Brigades foreign key contracts failed." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void BrigadesForeignKeyCount_MatchesP206Package()
    {
        var model = CreateModel();

        var brigadesForeignKeys = model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey => foreignKey.DeclaringEntityType.GetSchema() == "brigades")
            .Select(DescribeForeignKey)
            .Order(StringComparer.Ordinal)
            .ToArray();

        var expected = new[]
        {
            "brigades.brigade_services(BrigadeId) -> brigades.brigades(Id) DeleteBehavior=NoAction",
            "brigades.brigade_services(ServiceId) -> core.services(Id) DeleteBehavior=NoAction",
            "brigades.brigades(CommunityId) -> brigades.communities(Id) DeleteBehavior=NoAction",
            "brigades.brigades(MobileUnitId) -> brigades.mobile_units(Id) DeleteBehavior=NoAction",
            "brigades.brigades(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "brigades.communities(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "brigades.mobile_units(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction"
        };

        Assert.Equal(expected, brigadesForeignKeys);
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