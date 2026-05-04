using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class SyncEventTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePendingEvent()
    {
        var syncBatchId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var receivedAtServer = DateTimeOffset.UtcNow;

        var syncEvent = new SyncEvent(
            Guid.NewGuid(),
            syncBatchId,
            organizationId,
            "local-001",
            "Patient",
            SyncOperation.Create,
            """{ "firstName": "Carlos" }""",
            receivedAtServer: receivedAtServer);

        Assert.Equal(syncBatchId, syncEvent.SyncBatchId);
        Assert.Equal(organizationId, syncEvent.OrganizationId);
        Assert.Equal("local-001", syncEvent.LocalEventId);
        Assert.Equal("patient", syncEvent.EntityType);
        Assert.Equal(SyncOperation.Create, syncEvent.Operation);
        Assert.Equal(receivedAtServer, syncEvent.ReceivedAtServer);
        Assert.Equal(SyncEventStatus.Pending, syncEvent.Status);
        Assert.True(syncEvent.IsPending);
    }

    [Fact]
    public void Constructor_WithEmptyPayload_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new SyncEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "local-001",
                "patient",
                SyncOperation.Create,
                " "));
    }

    [Fact]
    public void MarkProcessing_WhenPending_ShouldSetProcessing()
    {
        var syncEvent = CreateEvent();

        syncEvent.MarkProcessing();

        Assert.Equal(SyncEventStatus.Processing, syncEvent.Status);
    }

    [Fact]
    public void Accept_ShouldSetAcceptedAndServerEntityId()
    {
        var syncEvent = CreateEvent();
        var processedAt = DateTimeOffset.UtcNow;
        var serverEntityId = Guid.NewGuid();

        syncEvent.Accept(processedAt, serverEntityId);

        Assert.Equal(SyncEventStatus.Accepted, syncEvent.Status);
        Assert.Equal(processedAt, syncEvent.ProcessedAt);
        Assert.Equal(serverEntityId, syncEvent.EntityId);
        Assert.True(syncEvent.IsAccepted);
    }

    [Fact]
    public void Reject_ShouldSetRejectedAndErrorMessage()
    {
        var syncEvent = CreateEvent();
        var processedAt = DateTimeOffset.UtcNow;

        syncEvent.Reject(processedAt, "Invalid payload");

        Assert.Equal(SyncEventStatus.Rejected, syncEvent.Status);
        Assert.Equal(processedAt, syncEvent.ProcessedAt);
        Assert.Equal("Invalid payload", syncEvent.ErrorMessage);
        Assert.True(syncEvent.IsRejected);
    }

    [Fact]
    public void MarkConflict_ShouldSetConflictAndReason()
    {
        var syncEvent = CreateEvent();
        var processedAt = DateTimeOffset.UtcNow;

        syncEvent.MarkConflict(processedAt, "Possible duplicate patient");

        Assert.Equal(SyncEventStatus.Conflict, syncEvent.Status);
        Assert.Equal(processedAt, syncEvent.ProcessedAt);
        Assert.Equal("Possible duplicate patient", syncEvent.ConflictReason);
        Assert.True(syncEvent.IsConflict);
    }

    [Fact]
    public void Reject_AfterAccepted_ShouldThrowDomainException()
    {
        var syncEvent = CreateEvent();

        syncEvent.Accept(DateTimeOffset.UtcNow, Guid.NewGuid());

        Assert.Throws<DomainException>(() =>
            syncEvent.Reject(DateTimeOffset.UtcNow, "No longer valid"));
    }

    private static SyncEvent CreateEvent()
    {
        return new SyncEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "local-001",
            "patient",
            SyncOperation.Create,
            "{}");
    }
}
