using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class CommunityTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateCommunity()
    {
        var organizationId = Guid.NewGuid();

        var community = new Community(
            Guid.NewGuid(),
            organizationId,
            " Monterrey ",
            " Obispado ",
            " Comunidad 1 ");

        Assert.Equal(organizationId, community.OrganizationId);
        Assert.Equal("Nuevo León", community.State);
        Assert.Equal("Monterrey", community.Municipality);
        Assert.Equal("Obispado", community.Colony);
        Assert.Equal("Comunidad 1", community.CommunityName);
        Assert.True(community.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyMunicipality_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Community(Guid.NewGuid(), Guid.NewGuid(), " "));
    }

    [Fact]
    public void UpdateRiskLevel_WithBlankValue_ShouldStoreNull()
    {
        var community = new Community(Guid.NewGuid(), Guid.NewGuid(), "Monterrey");

        community.UpdateRiskLevel(" ");

        Assert.Null(community.RiskLevel);
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveStatus()
    {
        var community = new Community(Guid.NewGuid(), Guid.NewGuid(), "Monterrey");

        community.Deactivate();

        Assert.Equal(CommunityStatus.Inactive, community.Status);
        Assert.False(community.IsActive);
    }
}
