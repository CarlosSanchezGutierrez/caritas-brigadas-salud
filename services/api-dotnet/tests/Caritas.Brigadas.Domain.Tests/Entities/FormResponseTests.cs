using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class FormResponseTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateDraft()
    {
        var response = new FormResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            """{ "field": "value" }""");

        Assert.Equal(FormResponseStatus.Draft, response.Status);
        Assert.Equal(SyncStatus.Synced, response.SyncStatus);
        Assert.False(response.IsCompleted);
    }

    [Fact]
    public void Constructor_WhenCreatedOffline_ShouldSetPendingSync()
    {
        var response = new FormResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "{}",
            createdOffline: true,
            deviceId: Guid.NewGuid());

        Assert.Equal(SyncStatus.Pending, response.SyncStatus);
    }

    [Fact]
    public void Complete_ShouldMarkCompleted()
    {
        var response = new FormResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "{}");

        var userId = Guid.NewGuid();
        var completedAt = DateTimeOffset.UtcNow;

        response.Complete(userId, completedAt, "HASH123");

        Assert.True(response.IsCompleted);
        Assert.Equal(userId, response.CompletedByUserId);
        Assert.Equal(completedAt, response.CompletedAt);
        Assert.Equal("HASH123", response.ResponseHash);
    }

    [Fact]
    public void UpdateResponse_AfterComplete_ShouldThrowDomainException()
    {
        var response = new FormResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "{}");

        response.Complete(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() =>
            response.UpdateResponse("""{ "x": 1 }"""));
    }
}
