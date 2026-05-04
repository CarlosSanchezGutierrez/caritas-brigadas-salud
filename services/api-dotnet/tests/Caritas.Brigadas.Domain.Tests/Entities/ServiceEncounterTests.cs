using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class ServiceEncounterTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveEncounter()
    {
        var organizationId = Guid.NewGuid();
        var visitId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow;

        var encounter = new ServiceEncounter(
            Guid.NewGuid(),
            organizationId,
            " enc-001 ",
            visitId,
            patientId,
            brigadeId,
            serviceId,
            providerId,
            startedAt);

        Assert.Equal(organizationId, encounter.OrganizationId);
        Assert.Equal("ENC-001", encounter.EncounterFolio);
        Assert.Equal(visitId, encounter.VisitId);
        Assert.Equal(patientId, encounter.PatientId);
        Assert.Equal(brigadeId, encounter.BrigadeId);
        Assert.Equal(serviceId, encounter.ServiceId);
        Assert.Equal(providerId, encounter.ProviderUserId);
        Assert.Equal(startedAt, encounter.StartedAt);
        Assert.Equal(EncounterStatus.Active, encounter.Status);
        Assert.Equal(SyncStatus.Synced, encounter.SyncStatus);
        Assert.True(encounter.IsActive);
    }

    [Fact]
    public void Constructor_WhenCreatedOffline_ShouldSetPendingSync()
    {
        var encounter = new ServiceEncounter(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "ENC-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            createdOffline: true,
            deviceId: Guid.NewGuid());

        Assert.True(encounter.CreatedOffline);
        Assert.Equal(SyncStatus.Pending, encounter.SyncStatus);
    }

    [Fact]
    public void Constructor_WithEmptyVisitId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new ServiceEncounter(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "ENC-001",
                Guid.Empty,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()));
    }

    [Fact]
    public void Start_WhenNotStarted_ShouldSetStartedAt()
    {
        var encounter = CreateEncounter();
        var startedAt = DateTimeOffset.UtcNow;

        encounter.Start(startedAt);

        Assert.Equal(startedAt, encounter.StartedAt);
    }

    [Fact]
    public void Start_WhenAlreadyStarted_ShouldThrowDomainException()
    {
        var encounter = CreateEncounter();
        var startedAt = DateTimeOffset.UtcNow;

        encounter.Start(startedAt);

        Assert.Throws<DomainException>(() =>
            encounter.Start(startedAt.AddMinutes(1)));
    }

    [Fact]
    public void UpdateClinicalSummary_WithValidData_ShouldUpdateFields()
    {
        var encounter = CreateEncounter();

        encounter.UpdateClinicalSummary(
            " Paciente refiere dolor leve ",
            " Tomar agua y seguimiento ");

        Assert.Equal("Paciente refiere dolor leve", encounter.NotesSummary);
        Assert.Equal("Tomar agua y seguimiento", encounter.Recommendations);
    }

    [Fact]
    public void UpdateClinicalSummary_WithBlankValues_ShouldStoreNulls()
    {
        var encounter = CreateEncounter();

        encounter.UpdateClinicalSummary(" ", "");

        Assert.Null(encounter.NotesSummary);
        Assert.Null(encounter.Recommendations);
    }

    [Fact]
    public void UpdateFollowUpAndReferral_ShouldUpdateFlags()
    {
        var encounter = CreateEncounter();

        encounter.UpdateFollowUpAndReferral(
            requiresFollowUp: true,
            requiresReferral: true);

        Assert.True(encounter.RequiresFollowUp);
        Assert.True(encounter.RequiresReferral);
    }

    [Fact]
    public void Complete_WhenActive_ShouldCompleteEncounter()
    {
        var encounter = CreateEncounter();
        var closedBy = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow;
        var endedAt = startedAt.AddMinutes(20);

        encounter.Start(startedAt);
        encounter.Complete(closedBy, endedAt);

        Assert.Equal(EncounterStatus.Completed, encounter.Status);
        Assert.Equal(endedAt, encounter.EndedAt);
        Assert.Equal(closedBy, encounter.ClosedByUserId);
        Assert.True(encounter.IsCompleted);
    }

    [Fact]
    public void Complete_WithEndBeforeStart_ShouldThrowDomainException()
    {
        var encounter = CreateEncounter();
        var startedAt = DateTimeOffset.UtcNow;

        encounter.Start(startedAt);

        Assert.Throws<DomainException>(() =>
            encounter.Complete(Guid.NewGuid(), startedAt.AddMinutes(-1)));
    }

    [Fact]
    public void MarkNeedsReview_WhenActive_ShouldSetNeedsReview()
    {
        var encounter = CreateEncounter();

        encounter.MarkNeedsReview();

        Assert.Equal(EncounterStatus.NeedsReview, encounter.Status);
        Assert.True(encounter.NeedsReview);
    }

    [Fact]
    public void UpdateSyncStatus_ShouldUpdateSyncStatus()
    {
        var encounter = CreateEncounter();

        encounter.UpdateSyncStatus(SyncStatus.Failed);

        Assert.Equal(SyncStatus.Failed, encounter.SyncStatus);
    }

    private static ServiceEncounter CreateEncounter()
    {
        return new ServiceEncounter(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "ENC-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());
    }
}
