namespace Caritas.Brigadas.Domain.Common;

public abstract class SoftDeletableEntity : Entity
{
    protected SoftDeletableEntity()
    {
    }

    protected SoftDeletableEntity(Guid id)
        : base(id)
    {
    }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    public void MarkAsDeleted(Guid? deletedByUserId, DateTimeOffset deletedAt)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedByUserId = deletedByUserId;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedByUserId = null;
    }
}
