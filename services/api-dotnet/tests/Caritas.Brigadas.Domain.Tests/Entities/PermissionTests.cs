using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class PermissionTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePermission()
    {
        var permission = new Permission(
            Guid.NewGuid(),
            " PATIENTS.READ ",
            "Leer pacientes",
            "patients",
            "read",
            "Permite consultar pacientes",
            PermissionSensitivity.Sensitive);

        Assert.Equal("patients.read", permission.Code);
        Assert.Equal("Leer pacientes", permission.Name);
        Assert.Equal("patients", permission.Module);
        Assert.Equal("read", permission.Action);
        Assert.Equal("Permite consultar pacientes", permission.Description);
        Assert.Equal(PermissionSensitivity.Sensitive, permission.SensitivityLevel);
        Assert.True(permission.IsSensitive);
    }

    [Fact]
    public void Constructor_WithCodeContainingSpaces_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Permission(
                Guid.NewGuid(),
                "patients read",
                "Leer pacientes",
                "patients",
                "read"));
    }

    [Fact]
    public void Constructor_WithNormalSensitivity_ShouldNotBeSensitive()
    {
        var permission = new Permission(
            Guid.NewGuid(),
            "reports.read",
            "Leer reportes",
            "reports",
            "read");

        Assert.False(permission.IsSensitive);
    }
}
