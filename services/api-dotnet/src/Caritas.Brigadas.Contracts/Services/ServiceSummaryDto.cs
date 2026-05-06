namespace Caritas.Brigadas.Contracts.Services;

public sealed record ServiceSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool RequiresConsent { get; init; }

    public bool RequiresClinicalNotes { get; init; }

    public bool RequiresFollowUpOption { get; init; }

    public bool RequiresReferralOption { get; init; }

    public bool IsSensitive { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
