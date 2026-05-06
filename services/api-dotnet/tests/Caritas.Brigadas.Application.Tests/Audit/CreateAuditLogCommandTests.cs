using Caritas.Brigadas.Application.Audit;
using Xunit;

namespace Caritas.Brigadas.Application.Tests.Audit;

public sealed class CreateAuditLogCommandTests
{
    [Fact]
    public void CreateAuditLogCommand_WhenInitialized_StoresValues()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var occurredAt = DateTimeOffset.UtcNow;

        var command = new CreateAuditLogCommand
        {
            OrganizationId = organizationId,
            UserId = userId,
            Action = "patients.create",
            EntityName = "Patient",
            EntityId = entityId,
            DetailsJson = "{\"source\":\"unit-test\"}",
            CorrelationId = "trace-123",
            IpAddress = "127.0.0.1",
            UserAgent = "unit-test",
            OccurredAtUtc = occurredAt
        };

        Assert.Equal(organizationId, command.OrganizationId);
        Assert.Equal(userId, command.UserId);
        Assert.Equal("patients.create", command.Action);
        Assert.Equal("Patient", command.EntityName);
        Assert.Equal(entityId, command.EntityId);
        Assert.Equal("{\"source\":\"unit-test\"}", command.DetailsJson);
        Assert.Equal("trace-123", command.CorrelationId);
        Assert.Equal("127.0.0.1", command.IpAddress);
        Assert.Equal("unit-test", command.UserAgent);
        Assert.Equal(occurredAt, command.OccurredAtUtc);
    }
}
