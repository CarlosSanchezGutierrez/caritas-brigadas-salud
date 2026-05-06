using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Tests.Common;

public sealed class SoftDeletableEntityTests
{
    [Fact]
    public void MarkAsDeleted_ShouldSetDeletionMetadata()
    {
        var userId = Guid.NewGuid();
        var deletedAt = DateTimeOffset.UtcNow;
        var entity = new TestSoftDeletableEntity(Guid.NewGuid());

        entity.MarkAsDeleted(userId, deletedAt);

        Assert.True(entity.IsDeleted);
        Assert.Equal(userId, entity.DeletedByUserId);
        Assert.Equal(deletedAt, entity.DeletedAt);
    }

    [Fact]
    public void Restore_ShouldClearDeletionMetadata()
    {
        var entity = new TestSoftDeletableEntity(Guid.NewGuid());

        entity.MarkAsDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow);
        entity.Restore();

        Assert.False(entity.IsDeleted);
        Assert.Null(entity.DeletedAt);
        Assert.Null(entity.DeletedByUserId);
    }

    private sealed class TestSoftDeletableEntity : SoftDeletableEntity
    {
        public TestSoftDeletableEntity(Guid id)
            : base(id)
        {
        }
    }
}
