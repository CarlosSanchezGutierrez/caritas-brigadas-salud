namespace Caritas.Brigadas.Domain.Entities;

public sealed class AuditLog
{
    private AuditLog()
    {
    }

    public AuditLog(
        Guid id,
        Guid organizationId,
        Guid? userId,
        string action,
        string entityName,
        Guid? entityId,
        string? detailsJson,
        string? correlationId,
        string? ipAddress,
        string? userAgent,
        DateTimeOffset occurredAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Audit log id is required.", nameof(id));
        }

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        Id = id;
        OrganizationId = organizationId;
        UserId = userId;
        Action = NormalizeRequired(action, nameof(action), 100);
        EntityName = NormalizeRequired(entityName, nameof(entityName), 150);
        EntityId = entityId;
        DetailsJson = NormalizeOptional(detailsJson);
        CorrelationId = NormalizeOptional(correlationId, 100);
        IpAddress = NormalizeOptional(ipAddress, 100);
        UserAgent = NormalizeOptional(userAgent, 500);
        OccurredAtUtc = occurredAtUtc;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid? UserId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string EntityName { get; private set; } = string.Empty;

    public Guid? EntityId { get; private set; }

    public string? DetailsJson { get; private set; }

    public string? CorrelationId { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    private static string NormalizeRequired(
        string value,
        string fieldName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} cannot exceed {maxLength} characters.", fieldName);
        }

        return normalized;
    }

    private static string? NormalizeOptional(
        string? value,
        int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (maxLength.HasValue && normalized.Length > maxLength.Value)
        {
            return normalized[..maxLength.Value];
        }

        return normalized;
    }
}
