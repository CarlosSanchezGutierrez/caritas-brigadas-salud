using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.MedicalReferrals;

public sealed record CreateMedicalReferralRequest
{
    public Guid EncounterId { get; init; }

    [MaxLength(50)]
    public string? ReferralFolio { get; init; }

    [MaxLength(250)]
    public string? DestinationInstitution { get; init; }

    [Required]
    [MaxLength(1000)]
    public string ReferralReason { get; init; } = string.Empty;

    [MaxLength(50)]
    public string? Priority { get; init; }

    public Guid? ReferredByUserId { get; init; }

    public Guid? ProviderSignatureId { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }
}