using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Tests.Common;

public sealed class AuditableEntityTests
{
    [Fact]
    public void MarkCreated_ShouldSetCreationMetadata()
    {
        var userId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var entity = new TestAuditableEntity(Guid.NewGuid());

        entity.MarkCreated(createdAt, userId);

        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(userId, entity.CreatedByUserId);
    }

    [Fact]
    public void MarkUpdated_ShouldSetUpdateMetadata()
    {
        var userId = Guid.NewGuid();
        var updatedAt = DateTimeOffset.UtcNow;
        var entity = new TestAuditableEntity(Guid.NewGuid());

        entity.MarkUpdated(updatedAt, userId);

        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(userId, entity.UpdatedByUserId);
    }

    private sealed class TestAuditableEntity : AuditableEntity
    {
        public TestAuditableEntity(Guid id)
            : base(id)
        {
        }
    }
}
