namespace Caritas.Brigadas.Application.Audit;

public sealed record CreateAuditLogCommand
{
    public Guid OrganizationId { get; init; }

    public Guid? UserId { get; init; }

    public string Action { get; init; } = string.Empty;

    public string EntityName { get; init; } = string.Empty;

    public Guid? EntityId { get; init; }

    public string? DetailsJson { get; init; }

    public string? CorrelationId { get; init; }

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }

    public DateTimeOffset? OccurredAtUtc { get; init; }
}
