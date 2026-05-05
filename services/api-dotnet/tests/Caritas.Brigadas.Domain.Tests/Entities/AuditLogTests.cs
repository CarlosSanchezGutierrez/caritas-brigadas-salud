using Caritas.Brigadas.Domain.Entities;
using Xunit;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class AuditLogTests
{
    [Fact]
    public void Constructor_WhenRequiredValuesAreValid_CreatesAuditLog()
    {
        var id = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var occurredAt = DateTimeOffset.UtcNow;

        var auditLog = new AuditLog(
            id,
            organizationId,
            userId,
            "patients.create",
            "Patient",
            entityId,
            "{\"field\":\"value\"}",
            "trace-123",
            "127.0.0.1",
            "unit-test",
            occurredAt);

        Assert.Equal(id, auditLog.Id);
        Assert.Equal(organizationId, auditLog.OrganizationId);
        Assert.Equal(userId, auditLog.UserId);
        Assert.Equal("patients.create", auditLog.Action);
        Assert.Equal("Patient", auditLog.EntityName);
        Assert.Equal(entityId, auditLog.EntityId);
        Assert.Equal("{\"field\":\"value\"}", auditLog.DetailsJson);
        Assert.Equal("trace-123", auditLog.CorrelationId);
        Assert.Equal("127.0.0.1", auditLog.IpAddress);
        Assert.Equal("unit-test", auditLog.UserAgent);
        Assert.Equal(occurredAt, auditLog.OccurredAtUtc);
    }

    [Fact]
    public void Constructor_WhenOrganizationIdIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new AuditLog(
                Guid.NewGuid(),
                Guid.Empty,
                null,
                "patients.create",
                "Patient",
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WhenActionIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new AuditLog(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                "",
                "Patient",
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WhenOptionalValuesAreWhitespace_NormalizesToNull()
    {
        var auditLog = new AuditLog(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "patients.create",
            "Patient",
            null,
            " ",
            " ",
            " ",
            " ",
            DateTimeOffset.UtcNow);

        Assert.Null(auditLog.DetailsJson);
        Assert.Null(auditLog.CorrelationId);
        Assert.Null(auditLog.IpAddress);
        Assert.Null(auditLog.UserAgent);
    }
}
