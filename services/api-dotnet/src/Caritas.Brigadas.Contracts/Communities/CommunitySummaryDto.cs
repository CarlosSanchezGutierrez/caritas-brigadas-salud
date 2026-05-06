namespace Caritas.Brigadas.Contracts.Communities;

public sealed record CommunitySummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string State { get; init; } = string.Empty;

    public string Municipality { get; init; } = string.Empty;

    public string? Colony { get; init; }

    public string? CommunityName { get; init; }

    public string? AddressReference { get; init; }

    public string? RiskLevel { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
