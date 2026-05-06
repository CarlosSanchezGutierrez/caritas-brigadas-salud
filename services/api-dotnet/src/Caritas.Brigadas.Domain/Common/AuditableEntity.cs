namespace Caritas.Brigadas.Domain.Common;

public abstract class AuditableEntity : SoftDeletableEntity
{
    protected AuditableEntity()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    protected AuditableEntity(Guid id)
        : base(id)
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public Guid? UpdatedByUserId { get; private set; }

    public void MarkCreated(DateTimeOffset createdAt, Guid? createdByUserId)
    {
        CreatedAt = createdAt;
        CreatedByUserId = createdByUserId;
    }

    public void MarkUpdated(DateTimeOffset updatedAt, Guid? updatedByUserId)
    {
        UpdatedAt = updatedAt;
        UpdatedByUserId = updatedByUserId;
    }
}
