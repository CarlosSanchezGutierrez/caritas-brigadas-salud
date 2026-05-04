using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Tests.Common;

public sealed class EntityTests
{
    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new TestEntity(Guid.Empty));
    }

    [Fact]
    public void Equals_WithSameTypeAndSameId_ShouldReturnTrue()
    {
        var id = Guid.NewGuid();

        var first = new TestEntity(id);
        var second = new TestEntity(id);

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_WithDifferentTypeAndSameId_ShouldReturnFalse()
    {
        var id = Guid.NewGuid();

        var first = new TestEntity(id);
        var second = new AnotherTestEntity(id);

        Assert.False(first.Equals(second));
    }

    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid id)
            : base(id)
        {
        }
    }

    private sealed class AnotherTestEntity : Entity
    {
        public AnotherTestEntity(Guid id)
            : base(id)
        {
        }
    }
}
