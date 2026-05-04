using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class AuditEventTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateAuditEvent()
    {
        var organizationId = Guid.NewGuid();
        var actorUserId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            " Patient ",
            " Patient.Created ",
            createdAt,
            actorUserId: actorUserId,
            entityId: entityId,
            metadataJson: """{ "source": "api" }""",
            ipAddress: "127.0.0.1");

        Assert.Equal(organizationId, auditEvent.OrganizationId);
        Assert.Equal(actorUserId, auditEvent.ActorUserId);
        Assert.Equal("patient", auditEvent.EntityType);
        Assert.Equal("patient.created", auditEvent.Action);
        Assert.Equal(entityId, auditEvent.EntityId);
        Assert.Equal("""{ "source": "api" }""", auditEvent.MetadataJson);
        Assert.Equal("127.0.0.1", auditEvent.IpAddress);
        Assert.Equal(createdAt, auditEvent.CreatedAt);
        Assert.False(auditEvent.HasIntegrityLink);
    }

    [Fact]
    public void Constructor_WithEmptyOrganizationId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AuditEvent(
                Guid.NewGuid(),
                Guid.Empty,
                "patient",
                AuditAction.PatientCreated,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithEmptyAction_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AuditEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "patient",
                " ",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AttachIntegrityHashes_ShouldSetHashes()
    {
        var auditEvent = CreateAuditEvent();

        auditEvent.AttachIntegrityHashes("HASH1", "HASH0");

        Assert.Equal("HASH1", auditEvent.EventHash);
        Assert.Equal("HASH0", auditEvent.PreviousEventHash);
        Assert.True(auditEvent.HasIntegrityLink);
    }

    private static AuditEvent CreateAuditEvent()
    {
        return new AuditEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "patient",
            AuditAction.PatientCreated,
            DateTimeOffset.UtcNow);
    }
}
