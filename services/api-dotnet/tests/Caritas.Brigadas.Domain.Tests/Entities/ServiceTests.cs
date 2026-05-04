using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class ServiceTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateService()
    {
        var organizationId = Guid.NewGuid();

        var service = new Service(
            Guid.NewGuid(),
            organizationId,
            " general_medicine ",
            " Consulta general ",
            " Salud ",
            " Atención médica general ",
            requiresConsent: true,
            requiresClinicalNotes: true,
            requiresFollowUpOption: true,
            requiresReferralOption: true,
            isSensitive: true);

        Assert.Equal(organizationId, service.OrganizationId);
        Assert.Equal(ServiceCode.GeneralMedicine, service.Code);
        Assert.Equal("Consulta general", service.Name);
        Assert.Equal("Salud", service.Category);
        Assert.Equal("Atención médica general", service.Description);
        Assert.True(service.RequiresConsent);
        Assert.True(service.RequiresClinicalNotes);
        Assert.True(service.RequiresFollowUpOption);
        Assert.True(service.RequiresReferralOption);
        Assert.True(service.IsSensitive);
        Assert.Equal(ServiceStatus.Active, service.Status);
        Assert.True(service.IsActive);
    }

    [Fact]
    public void Constructor_WithCodeContainingSpaces_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Service(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "GENERAL MEDICINE",
                "Consulta general",
                "Salud"));
    }

    [Fact]
    public void UpdateDetails_WithBlankDescription_ShouldSetDescriptionNull()
    {
        var service = new Service(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ServiceCode.Dentistry,
            "Odontología",
            "Salud");

        service.UpdateDetails(" Odontología comunitaria ", " Salud dental ", " ");

        Assert.Equal("Odontología comunitaria", service.Name);
        Assert.Equal("Salud dental", service.Category);
        Assert.Null(service.Description);
    }

    [Fact]
    public void UpdateRules_ShouldUpdateServiceRules()
    {
        var service = new Service(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ServiceCode.Psychology,
            "Psicología",
            "Salud mental");

        service.UpdateRules(
            requiresConsent: true,
            requiresClinicalNotes: true,
            requiresFollowUpOption: true,
            requiresReferralOption: false,
            isSensitive: true);

        Assert.True(service.RequiresConsent);
        Assert.True(service.RequiresClinicalNotes);
        Assert.True(service.RequiresFollowUpOption);
        Assert.False(service.RequiresReferralOption);
        Assert.True(service.IsSensitive);
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveStatus()
    {
        var service = new Service(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ServiceCode.Optometry,
            "Optometría",
            "Salud visual");

        service.Deactivate();

        Assert.Equal(ServiceStatus.Inactive, service.Status);
        Assert.False(service.IsActive);
    }
}
