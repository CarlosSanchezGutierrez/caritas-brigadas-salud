using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Domain;

public sealed class SyncEventIdempotencyDomainTests
{
    [Fact]
    public void Constructor_StoresExplicitIdempotencyKey()
    {
        var syncEvent = new SyncEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "local-event-001",
            "vital_signs",
            SyncOperation.Create,
            """{""value"":true}""",
            idempotencyKey: "org-device-local-event-001");

        Assert.Equal("local-event-001", syncEvent.LocalEventId);
        Assert.Equal("org-device-local-event-001", syncEvent.IdempotencyKey);
    }

    [Fact]
    public void Constructor_FallsBackToLocalEventIdWhenIdempotencyKeyIsNotProvided()
    {
        var syncEvent = new SyncEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "local-event-002",
            "patient",
            SyncOperation.Create,
            """{""value"":true}""");

        Assert.Equal("local-event-002", syncEvent.IdempotencyKey);
    }

    [Fact]
    public void Constructor_RejectsEmptyLocalEventId()
    {
        Assert.Throws<DomainException>(() =>
            new SyncEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                " ",
                "patient",
                SyncOperation.Create,
                """{""value"":true}"""));
    }

    [Fact]
    public void Constructor_RejectsOverlongIdempotencyKey()
    {
        var overlong = new string('x', 251);

        Assert.Throws<DomainException>(() =>
            new SyncEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "local-event-003",
                "patient",
                SyncOperation.Create,
                """{""value"":true}""",
                idempotencyKey: overlong));
    }
}