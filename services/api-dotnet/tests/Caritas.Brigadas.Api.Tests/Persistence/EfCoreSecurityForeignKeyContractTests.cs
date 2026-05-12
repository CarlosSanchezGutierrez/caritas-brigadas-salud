using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfCoreSecurityForeignKeyContractTests
{
    [Fact]
    public void CoreSecurityForeignKeys_AreConfiguredWithNoActionDeleteBehavior()
    {
        var model = CreateModel();

        var expectations = new[]
        {
            ForeignKeyExpectation.Required<Role, Organization>(nameof(Role.OrganizationId)),
            ForeignKeyExpectation.Required<User, Organization>(nameof(User.OrganizationId)),
            ForeignKeyExpectation.Required<UserRole, Organization>(nameof(UserRole.OrganizationId)),
            ForeignKeyExpectation.Required<UserRole, User>(nameof(UserRole.UserId)),
            ForeignKeyExpectation.Required<UserRole, Role>(nameof(UserRole.RoleId)),
            ForeignKeyExpectation.Required<RolePermission, Role>(nameof(RolePermission.RoleId)),
            ForeignKeyExpectation.Required<RolePermission, Permission>(nameof(RolePermission.PermissionId)),
            ForeignKeyExpectation.Required<Service, Organization>(nameof(Service.OrganizationId))
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
            "Core/security foreign key contracts failed." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void CoreSecurityForeignKeyCount_MatchesP205Package()
    {
        var model = CreateModel();

        var coreSecurityForeignKeys = model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey =>
                foreignKey.DeclaringEntityType.GetSchema() == "core" &&
                foreignKey.PrincipalEntityType.GetSchema() == "core")
            .Select(DescribeForeignKey)
            .Order(StringComparer.Ordinal)
            .ToArray();

        var expected = new[]
        {
            "core.role_permissions(PermissionId) -> core.permissions(Id) DeleteBehavior=NoAction",
            "core.role_permissions(RoleId) -> core.roles(Id) DeleteBehavior=NoAction",
            "core.roles(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "core.services(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "core.user_roles(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction",
            "core.user_roles(RoleId) -> core.roles(Id) DeleteBehavior=NoAction",
            "core.user_roles(UserId) -> core.users(Id) DeleteBehavior=NoAction",
            "core.users(OrganizationId) -> core.organizations(Id) DeleteBehavior=NoAction"
        };

        Assert.Equal(expected, coreSecurityForeignKeys);
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