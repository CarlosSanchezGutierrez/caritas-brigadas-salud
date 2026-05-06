namespace Caritas.Brigadas.Contracts.FormTemplates;

public sealed record FormTemplateSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid ServiceId { get; init; }

    public string FormCode { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public string SchemaJson { get; init; } = string.Empty;

    public string? UiSchemaJson { get; init; }

    public string? ValidationRulesJson { get; init; }

    public bool IsActive { get; init; }
}
