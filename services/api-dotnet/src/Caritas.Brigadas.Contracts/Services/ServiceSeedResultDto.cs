namespace Caritas.Brigadas.Contracts.Services;

public sealed record ServiceSeedResultDto
{
    public Guid OrganizationId { get; init; }

    public int ServicesCreated { get; init; }

    public IReadOnlyCollection<string> ServiceCodes { get; init; } = Array.Empty<string>();
}
