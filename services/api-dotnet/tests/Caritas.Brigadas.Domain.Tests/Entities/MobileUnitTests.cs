using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class MobileUnitTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateMobileUnit()
    {
        var organizationId = Guid.NewGuid();

        var unit = new MobileUnit(
            Guid.NewGuid(),
            organizationId,
            " Camión Médico 1 ",
            " Unidad médica ",
            " ABC-123 ",
            " Unidad para brigadas ");

        Assert.Equal(organizationId, unit.OrganizationId);
        Assert.Equal("Camión Médico 1", unit.Name);
        Assert.Equal("Unidad médica", unit.UnitType);
        Assert.Equal("ABC-123", unit.PlateNumber);
        Assert.Equal("Unidad para brigadas", unit.Description);
        Assert.True(unit.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new MobileUnit(Guid.NewGuid(), Guid.NewGuid(), " "));
    }

    [Fact]
    public void UpdateDetails_WithBlankOptionals_ShouldStoreNulls()
    {
        var unit = new MobileUnit(Guid.NewGuid(), Guid.NewGuid(), "Unidad");

        unit.UpdateDetails("Unidad 2", " ", "", null);

        Assert.Equal("Unidad 2", unit.Name);
        Assert.Null(unit.UnitType);
        Assert.Null(unit.PlateNumber);
        Assert.Null(unit.Description);
    }
}
