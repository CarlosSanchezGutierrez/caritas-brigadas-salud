namespace Caritas.Brigadas.Contracts.FormTemplates;

public sealed record FormTemplateSeedResultDto
{
    public Guid OrganizationId { get; init; }

    public int FormTemplatesCreated { get; init; }

    public IReadOnlyCollection<string> FormCodes { get; init; } = Array.Empty<string>();
}
