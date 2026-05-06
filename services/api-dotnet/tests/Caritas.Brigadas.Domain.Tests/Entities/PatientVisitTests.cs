using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class PatientVisitTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveVisit()
    {
        var organizationId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var registeredByUserId = Guid.NewGuid();
        var arrivalTime = DateTimeOffset.UtcNow;

        var visit = new PatientVisit(
            Guid.NewGuid(),
            organizationId,
            " visit-001 ",
            patientId,
            brigadeId,
            arrivalTime,
            registeredByUserId);

        Assert.Equal(organizationId, visit.OrganizationId);
        Assert.Equal("VISIT-001", visit.VisitFolio);
        Assert.Equal(patientId, visit.PatientId);
        Assert.Equal(brigadeId, visit.BrigadeId);
        Assert.Equal(arrivalTime, visit.ArrivalTime);
        Assert.Equal(registeredByUserId, visit.RegisteredByUserId);
        Assert.Equal(VisitStatus.Active, visit.VisitStatus);
        Assert.Equal(SyncStatus.Synced, visit.SyncStatus);
        Assert.True(visit.IsActive);
    }

    [Fact]
    public void Constructor_WhenCreatedOffline_ShouldSetPendingSync()
    {
        var visit = new PatientVisit(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "VISIT-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            createdOffline: true,
            deviceId: Guid.NewGuid());

        Assert.True(visit.CreatedOffline);
        Assert.Equal(SyncStatus.Pending, visit.SyncStatus);
    }

    [Fact]
    public void Constructor_WithEmptyPatientId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new PatientVisit(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "VISIT-001",
                Guid.Empty,
                Guid.NewGuid()));
    }

    [Fact]
    public void Close_WhenActive_ShouldCloseVisit()
    {
        var visit = CreateVisit();
        var closedBy = Guid.NewGuid();
        var closedAt = DateTimeOffset.UtcNow;

        visit.Close(closedBy, closedAt);

        Assert.Equal(VisitStatus.Closed, visit.VisitStatus);
        Assert.Equal(closedAt, visit.ClosedAt);
        Assert.Equal(closedBy, visit.ClosedByUserId);
        Assert.True(visit.IsClosed);
    }

    [Fact]
    public void Close_WhenCancelled_ShouldThrowDomainException()
    {
        var visit = CreateVisit();

        visit.Cancel();

        Assert.Throws<DomainException>(() =>
            visit.Close(Guid.NewGuid(), DateTimeOffset.UtcNow));
    }

    [Fact]
    public void MarkNeedsReview_WhenActive_ShouldSetNeedsReview()
    {
        var visit = CreateVisit();

        visit.MarkNeedsReview();

        Assert.Equal(VisitStatus.NeedsReview, visit.VisitStatus);
        Assert.True(visit.NeedsReview);
    }

    [Fact]
    public void UpdateSyncStatus_ShouldUpdateSyncStatus()
    {
        var visit = CreateVisit();

        visit.UpdateSyncStatus(SyncStatus.Conflict);

        Assert.Equal(SyncStatus.Conflict, visit.SyncStatus);
    }

    private static PatientVisit CreateVisit()
    {
        return new PatientVisit(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "VISIT-001",
            Guid.NewGuid(),
            Guid.NewGuid());
    }
}
