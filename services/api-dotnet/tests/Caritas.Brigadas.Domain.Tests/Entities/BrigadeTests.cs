using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class BrigadeTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePlannedBrigade()
    {
        var organizationId = Guid.NewGuid();
        var scheduledDate = new DateOnly(2026, 4, 28);

        var brigade = new Brigade(
            Guid.NewGuid(),
            organizationId,
            " Brigada Obispado ",
            " Salud ",
            scheduledDate,
            municipality: " Monterrey ",
            colony: " Obispado ");

        Assert.Equal(organizationId, brigade.OrganizationId);
        Assert.Equal("Brigada Obispado", brigade.Name);
        Assert.Equal("Salud", brigade.BrigadeType);
        Assert.Equal(scheduledDate, brigade.ScheduledDate);
        Assert.Equal("Monterrey", brigade.Municipality);
        Assert.Equal("Obispado", brigade.Colony);
        Assert.Equal(BrigadeState.Planned, brigade.Status);
        Assert.True(brigade.IsPlanned);
    }

    [Fact]
    public void Open_WhenPlanned_ShouldSetActiveStatus()
    {
        var brigade = CreateBrigade();
        var userId = Guid.NewGuid();
        var openedAt = DateTimeOffset.UtcNow;

        brigade.Open(userId, openedAt);

        Assert.Equal(BrigadeState.Active, brigade.Status);
        Assert.Equal(openedAt, brigade.OpenedAt);
        Assert.Equal(userId, brigade.OpenedByUserId);
        Assert.True(brigade.IsActive);
    }

    [Fact]
    public void Close_WhenActive_ShouldSetClosedStatus()
    {
        var brigade = CreateBrigade();
        var openedAt = DateTimeOffset.UtcNow;
        var closedAt = openedAt.AddHours(4);

        brigade.Open(Guid.NewGuid(), openedAt);
        brigade.Close(Guid.NewGuid(), closedAt);

        Assert.Equal(BrigadeState.Closed, brigade.Status);
        Assert.Equal(closedAt, brigade.ClosedAt);
        Assert.True(brigade.IsClosed);
    }

    [Fact]
    public void Close_WhenPlanned_ShouldThrowDomainException()
    {
        var brigade = CreateBrigade();

        Assert.Throws<DomainException>(() =>
            brigade.Close(Guid.NewGuid(), DateTimeOffset.UtcNow));
    }

    [Fact]
    public void MarkSynced_WhenClosed_ShouldSetSyncedStatus()
    {
        var brigade = CreateBrigade();

        brigade.Open(Guid.NewGuid(), DateTimeOffset.UtcNow);
        brigade.Close(Guid.NewGuid(), DateTimeOffset.UtcNow.AddHours(4));
        brigade.MarkSynced();

        Assert.Equal(BrigadeState.Synced, brigade.Status);
    }

    [Fact]
    public void Cancel_WhenPlanned_ShouldSetCancelledStatus()
    {
        var brigade = CreateBrigade();

        brigade.Cancel();

        Assert.Equal(BrigadeState.Cancelled, brigade.Status);
    }

    private static Brigade CreateBrigade()
    {
        return new Brigade(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Brigada",
            "Salud",
            DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
