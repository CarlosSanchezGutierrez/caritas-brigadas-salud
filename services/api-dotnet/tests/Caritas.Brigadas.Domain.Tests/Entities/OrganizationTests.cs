using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class OrganizationTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateOrganization()
    {
        var id = Guid.NewGuid();

        var organization = new Organization(
            id,
            " Cáritas de Monterrey ",
            " Cáritas de Monterrey, A.B.P. ",
            " RFC123 ");

        Assert.Equal(id, organization.Id);
        Assert.Equal("Cáritas de Monterrey", organization.Name);
        Assert.Equal("Cáritas de Monterrey, A.B.P.", organization.LegalName);
        Assert.Equal("RFC123", organization.Rfc);
        Assert.Equal(OrganizationStatus.Active, organization.Status);
        Assert.True(organization.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Organization(Guid.NewGuid(), " "));
    }

    [Fact]
    public void UpdateIdentity_WithValidData_ShouldUpdateFields()
    {
        var organization = new Organization(Guid.NewGuid(), "Original");

        organization.UpdateIdentity(
            "Cáritas Monterrey",
            "Cáritas de Monterrey, A.B.P.",
            "ABC123");

        Assert.Equal("Cáritas Monterrey", organization.Name);
        Assert.Equal("Cáritas de Monterrey, A.B.P.", organization.LegalName);
        Assert.Equal("ABC123", organization.Rfc);
    }

    [Fact]
    public void UpdateContact_WithBlankOptionalFields_ShouldStoreNulls()
    {
        var organization = new Organization(Guid.NewGuid(), "Cáritas");

        organization.UpdateContact(" ", "", null, "   ");

        Assert.Null(organization.Address);
        Assert.Null(organization.Phone);
        Assert.Null(organization.Email);
        Assert.Null(organization.Website);
    }

    [Fact]
    public void UpdateBranding_WithValidData_ShouldUpdateBrandingFields()
    {
        var organization = new Organization(Guid.NewGuid(), "Cáritas");

        organization.UpdateBranding(
            "https://example.com/logo.png",
            "#009CA6",
            "#003B5C",
            "#FF7F32",
            "Gotham");

        Assert.Equal("https://example.com/logo.png", organization.LogoUrl);
        Assert.Equal("#009CA6", organization.PrimaryColor);
        Assert.Equal("#003B5C", organization.SecondaryColor);
        Assert.Equal("#FF7F32", organization.AccentColor);
        Assert.Equal("Gotham", organization.FontFamily);
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveStatus()
    {
        var organization = new Organization(Guid.NewGuid(), "Cáritas");

        organization.Deactivate();

        Assert.Equal(OrganizationStatus.Inactive, organization.Status);
        Assert.False(organization.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetActiveStatus()
    {
        var organization = new Organization(Guid.NewGuid(), "Cáritas");

        organization.Deactivate();
        organization.Activate();

        Assert.Equal(OrganizationStatus.Active, organization.Status);
        Assert.True(organization.IsActive);
    }
}
