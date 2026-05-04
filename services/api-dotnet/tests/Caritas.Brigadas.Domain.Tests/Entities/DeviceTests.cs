using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class DeviceTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateDevice()
    {
        var organizationId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();

        var device = new Device(
            Guid.NewGuid(),
            organizationId,
            " Tablet ",
            " IOS ",
            " Institutional ",
            " iPad Brigada 1 ",
            assignedUserId);

        Assert.Equal(organizationId, device.OrganizationId);
        Assert.Equal(DeviceType.Tablet, device.DeviceType);
        Assert.Equal(DevicePlatform.Ios, device.Platform);
        Assert.Equal(DeviceOwnerType.Institutional, device.OwnerType);
        Assert.Equal("iPad Brigada 1", device.DeviceName);
        Assert.Equal(assignedUserId, device.AssignedToUserId);
        Assert.False(device.IsApproved);
        Assert.False(device.IsRevoked);
        Assert.False(device.CanSync);
    }

    [Fact]
    public void Constructor_WithEmptyOrganizationId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Device(
                Guid.NewGuid(),
                Guid.Empty,
                DeviceType.Tablet,
                DevicePlatform.Ios,
                DeviceOwnerType.Institutional));
    }

    [Fact]
    public void Approve_ShouldAllowSync()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeviceType.Tablet,
            DevicePlatform.Ios,
            DeviceOwnerType.Institutional);

        var approvedBy = Guid.NewGuid();
        var approvedAt = DateTimeOffset.UtcNow;

        device.Approve(approvedBy, approvedAt);

        Assert.True(device.IsApproved);
        Assert.Equal(approvedBy, device.ApprovedByUserId);
        Assert.Equal(approvedAt, device.ApprovedAt);
        Assert.True(device.CanSync);
    }

    [Fact]
    public void Revoke_ShouldBlockSync()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeviceType.Tablet,
            DevicePlatform.Ios,
            DeviceOwnerType.Institutional);

        device.Approve(Guid.NewGuid(), DateTimeOffset.UtcNow);
        device.Revoke(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.True(device.IsRevoked);
        Assert.False(device.CanSync);
    }

    [Fact]
    public void MarkSynced_WhenDeviceIsNotApproved_ShouldThrowDomainException()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeviceType.Phone,
            DevicePlatform.Android,
            DeviceOwnerType.Personal);

        Assert.Throws<DomainException>(() =>
            device.MarkSynced(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void MarkSynced_WhenApproved_ShouldSetLastSyncAt()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeviceType.Phone,
            DevicePlatform.Android,
            DeviceOwnerType.Personal);

        var syncAt = DateTimeOffset.UtcNow;

        device.Approve(Guid.NewGuid(), DateTimeOffset.UtcNow);
        device.MarkSynced(syncAt);

        Assert.Equal(syncAt, device.LastSyncAt);
    }
}
