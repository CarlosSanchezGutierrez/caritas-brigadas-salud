using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.MedicationDeliveries;

public sealed record CreateMedicationDeliveryRequest
{
    public Guid EncounterId { get; init; }

    [Required]
    [MaxLength(250)]
    public string MedicationName { get; init; } = string.Empty;

    [MaxLength(150)]
    public string? Presentation { get; init; }

    [MaxLength(100)]
    public string? Quantity { get; init; }

    [MaxLength(100)]
    public string? LotNumber { get; init; }

    public DateOnly? ExpirationDate { get; init; }

    [MaxLength(1000)]
    public string? Instructions { get; init; }

    public Guid? DeliveredByUserId { get; init; }

    [MaxLength(250)]
    public string? ReceivedByName { get; init; }

    public Guid? SignatureId { get; init; }

    public bool MarkAsDelivered { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }
}