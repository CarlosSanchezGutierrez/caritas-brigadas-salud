namespace Caritas.Brigadas.Contracts.Audit;

public sealed record AuditLogSummaryDto
{
    public Guid Id { get; init; }

    public Guid? OrganizationId { get; init; }

    public string EntityName { get; init; } = string.Empty;

    public Guid? EntityId { get; init; }

    public string Action { get; init; } = string.Empty;

    public Guid? UserId { get; init; }

    public DateTimeOffset? OccurredAtUtc { get; init; }

    public string? CorrelationId { get; init; }

    public string? IpAddress { get; init; }

    public string? DetailsJson { get; init; }
}
