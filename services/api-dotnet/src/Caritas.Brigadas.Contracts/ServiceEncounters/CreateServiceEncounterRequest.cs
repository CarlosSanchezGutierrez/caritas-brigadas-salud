using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.ServiceEncounters;

public sealed record CreateServiceEncounterRequest
{
    [MaxLength(50)]
    public string? EncounterFolio { get; init; }

    public Guid VisitId { get; init; }

    [Required]
    [MaxLength(100)]
    public string ServiceCode { get; init; } = string.Empty;

    public Guid? ProviderUserId { get; init; }

    public DateTimeOffset? StartedAt { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }
}
