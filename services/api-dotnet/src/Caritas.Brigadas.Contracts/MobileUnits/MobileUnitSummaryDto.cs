namespace Caritas.Brigadas.Contracts.MobileUnits;

public sealed record MobileUnitSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? UnitType { get; init; }

    public string? PlateNumber { get; init; }

    public string? Description { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
