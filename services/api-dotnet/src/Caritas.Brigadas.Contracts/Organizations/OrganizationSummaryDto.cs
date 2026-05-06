namespace Caritas.Brigadas.Contracts.Organizations;

public sealed record OrganizationSummaryDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? LegalName { get; init; }

    public string? Rfc { get; init; }

    public string? Email { get; init; }

    public string? Website { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
