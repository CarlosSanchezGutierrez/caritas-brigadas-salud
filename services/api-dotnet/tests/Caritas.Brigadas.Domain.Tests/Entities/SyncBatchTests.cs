using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class SyncBatchTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateReceivedBatch()
    {
        var organizationId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow;

        var batch = new SyncBatch(
            Guid.NewGuid(),
            organizationId,
            deviceId,
            userId,
            startedAt,
            brigadeId,
            eventsCount: 3);

        Assert.Equal(organizationId, batch.OrganizationId);
        Assert.Equal(deviceId, batch.DeviceId);
        Assert.Equal(userId, batch.UserId);
        Assert.Equal(brigadeId, batch.BrigadeId);
        Assert.Equal(startedAt, batch.StartedAt);
        Assert.Equal(3, batch.EventsCount);
        Assert.Equal(SyncBatchStatus.Received, batch.Status);
        Assert.False(batch.IsCompleted);
    }

    [Fact]
    public void Constructor_WithNegativeEventsCount_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new SyncBatch(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                eventsCount: -1));
    }

    [Fact]
    public void MarkProcessing_WhenReceived_ShouldSetProcessing()
    {
        var batch = CreateBatch(eventsCount: 2);

        batch.MarkProcessing();

        Assert.Equal(SyncBatchStatus.Processing, batch.Status);
    }

    [Fact]
    public void Complete_WithNoErrors_ShouldSetCompleted()
    {
        var batch = CreateBatch(eventsCount: 2);
        var completedAt = DateTimeOffset.UtcNow;

        batch.MarkProcessing();
        batch.Complete(
            completedAt,
            acceptedCount: 2,
            rejectedCount: 0,
            conflictCount: 0);

        Assert.Equal(SyncBatchStatus.Completed, batch.Status);
        Assert.Equal(completedAt, batch.CompletedAt);
        Assert.True(batch.IsCompleted);
    }

    [Fact]
    public void Complete_WithRejectedEvents_ShouldSetCompletedWithErrors()
    {
        var batch = CreateBatch(eventsCount: 3);

        batch.Complete(
            DateTimeOffset.UtcNow,
            acceptedCount: 2,
            rejectedCount: 1,
            conflictCount: 0,
            errorSummary: "One event failed");

        Assert.Equal(SyncBatchStatus.CompletedWithErrors, batch.Status);
        Assert.Equal("One event failed", batch.ErrorSummary);
        Assert.True(batch.IsCompleted);
    }

    [Fact]
    public void Complete_WithProcessedCountGreaterThanTotal_ShouldThrowDomainException()
    {
        var batch = CreateBatch(eventsCount: 1);

        Assert.Throws<DomainException>(() =>
            batch.Complete(
                DateTimeOffset.UtcNow,
                acceptedCount: 2,
                rejectedCount: 0,
                conflictCount: 0));
    }

    [Fact]
    public void Fail_ShouldSetFailedStatus()
    {
        var batch = CreateBatch(eventsCount: 2);
        var completedAt = DateTimeOffset.UtcNow;

        batch.Fail(completedAt, "SQL timeout");

        Assert.Equal(SyncBatchStatus.Failed, batch.Status);
        Assert.Equal(completedAt, batch.CompletedAt);
        Assert.Equal("SQL timeout", batch.ErrorSummary);
    }

    private static SyncBatch CreateBatch(int eventsCount)
    {
        return new SyncBatch(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            eventsCount: eventsCount);
    }
}
