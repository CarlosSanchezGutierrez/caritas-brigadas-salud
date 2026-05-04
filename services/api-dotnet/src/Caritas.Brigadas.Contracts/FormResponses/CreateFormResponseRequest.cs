using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.FormResponses;

public sealed record CreateFormResponseRequest
{
    public Guid EncounterId { get; init; }

    public Guid FormTemplateId { get; init; }

    [Required]
    public string ResponseJson { get; init; } = string.Empty;

    public Guid? SubmittedByUserId { get; init; }

    public DateTimeOffset? SubmittedAt { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }
}
