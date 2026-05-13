using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfVitalSignsRecordContractTests
{
    [Fact]
    public void VitalSignsRecord_IsMappedToClinicalSchemaWithCanonicalUnitFields()
    {
        var entity = CreateModel().FindEntityType(typeof(VitalSignsRecord));

        Assert.NotNull(entity);
        Assert.Equal("clinical", entity!.GetSchema());
        Assert.Equal("vital_signs", entity.GetTableName());

        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.SystolicBloodPressureMmHg)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.DiastolicBloodPressureMmHg)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.HeartRateBpm)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.RespiratoryRatePerMinute)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.TemperatureCelsius)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.OxygenSaturationPercent)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.WeightKg)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.HeightCm)));
        Assert.NotNull(entity.FindProperty(nameof(VitalSignsRecord.GlucoseMgDl)));
    }

    [Fact]
    public void VitalSignsRecord_ForeignKeysUseNoActionAndExpectedOptionality()
    {
        var entity = CreateModel().FindEntityType(typeof(VitalSignsRecord));

        Assert.NotNull(entity);

        var expected = new[]
        {
            "EncounterId -> ServiceEncounter Required=False DeleteBehavior=NoAction",
            "MeasuredByUserId -> User Required=False DeleteBehavior=NoAction",
            "OrganizationId -> Organization Required=True DeleteBehavior=NoAction",
            "PatientId -> Patient Required=True DeleteBehavior=NoAction",
            "VisitId -> PatientVisit Required=True DeleteBehavior=NoAction"
        };

        var actual = entity!
            .GetForeignKeys()
            .Select(foreignKey =>
            {
                var propertyNames = string.Join(", ", foreignKey.Properties.Select(property => property.Name));
                var principalName = foreignKey.PrincipalEntityType.ClrType.Name;

                return $"{propertyNames} -> {principalName} Required={!foreignKey.Properties.Any(property => property.IsNullable)} DeleteBehavior={foreignKey.DeleteBehavior}";
            })
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, actual);
    }

    private static Microsoft.EntityFrameworkCore.Metadata.IModel CreateModel()
    {
        var options = new DbContextOptionsBuilder<CaritasDbContext>()
            .UseSqlServer("Server=localhost;Database=Caritas_ModelOnly;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var dbContext = new CaritasDbContext(options);

        return dbContext.Model;
    }
}