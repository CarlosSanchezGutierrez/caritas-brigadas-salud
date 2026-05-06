using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class UserRoleTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateAssignment()
    {
        var assignedAt = DateTimeOffset.UtcNow;

        var assignment = new UserRole(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            assignedAt,
            Guid.NewGuid(),
            assignedAt.AddDays(1));

        Assert.Equal(UserRoleStatus.Active, assignment.Status);
        Assert.True(assignment.IsActiveAt(assignedAt.AddMinutes(1)));
    }

    [Fact]
    public void Constructor_WithExpiredDateBeforeAssignedAt_ShouldThrowDomainException()
    {
        var assignedAt = DateTimeOffset.UtcNow;

        Assert.Throws<DomainException>(() =>
            new UserRole(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                assignedAt,
                expiresAt: assignedAt.AddMinutes(-1)));
    }

    [Fact]
    public void Revoke_ShouldDisableAssignment()
    {
        var assignedAt = DateTimeOffset.UtcNow;

        var assignment = new UserRole(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            assignedAt);

        assignment.Revoke();

        Assert.Equal(UserRoleStatus.Revoked, assignment.Status);
        Assert.False(assignment.IsActiveAt(assignedAt.AddMinutes(1)));
    }
}
