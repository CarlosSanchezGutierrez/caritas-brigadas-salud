using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class BrigadeServiceTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateAvailableBrigadeService()
    {
        var brigadeId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var leadUserId = Guid.NewGuid();

        var brigadeService = new BrigadeService(
            Guid.NewGuid(),
            brigadeId,
            serviceId,
            capacityEstimate: 50,
            assignedLeadUserId: leadUserId);

        Assert.Equal(brigadeId, brigadeService.BrigadeId);
        Assert.Equal(serviceId, brigadeService.ServiceId);
        Assert.True(brigadeService.IsAvailable);
        Assert.Equal(50, brigadeService.CapacityEstimate);
        Assert.Equal(leadUserId, brigadeService.AssignedLeadUserId);
    }

    [Fact]
    public void Constructor_WithNegativeCapacity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new BrigadeService(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                capacityEstimate: -1));
    }

    [Fact]
    public void MarkUnavailable_ShouldSetUnavailable()
    {
        var brigadeService = new BrigadeService(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        brigadeService.MarkUnavailable();

        Assert.False(brigadeService.IsAvailable);
    }
}
