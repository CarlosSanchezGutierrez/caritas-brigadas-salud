using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        var organizationId = Guid.NewGuid();

        var user = new User(
            Guid.NewGuid(),
            organizationId,
            " Carlos Sánchez ",
            " CARLOS@EXAMPLE.COM ",
            " 8112345678 ",
            " carlos ");

        Assert.Equal(organizationId, user.OrganizationId);
        Assert.Equal("Carlos Sánchez", user.FullName);
        Assert.Equal("carlos@example.com", user.Email);
        Assert.Equal("8112345678", user.Phone);
        Assert.Equal("carlos", user.Username);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyOrganizationId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new User(Guid.NewGuid(), Guid.Empty, "Carlos"));
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveStatus()
    {
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "Carlos");
        var deactivatedAt = DateTimeOffset.UtcNow;

        user.Deactivate(deactivatedAt);

        Assert.Equal(UserStatus.Inactive, user.Status);
        Assert.Equal(deactivatedAt, user.DeactivatedAt);
        Assert.False(user.IsActive);
    }
}
