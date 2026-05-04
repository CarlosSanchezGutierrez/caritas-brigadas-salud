using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.ConsentDocuments;

public sealed record CreateConsentDocumentRequest
{
    public Guid PatientId { get; init; }

    public Guid? VisitId { get; init; }

    [Required]
    [MaxLength(100)]
    public string ConsentType { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DocumentVersion { get; init; } = "1.0.0";

    [MaxLength(10000)]
    public string? DocumentTextSnapshot { get; init; }

    public string? SignatureDataUrl { get; init; }

    [MaxLength(200)]
    public string? GuardianFullName { get; init; }

    [MaxLength(100)]
    public string? GuardianRelationship { get; init; }

    public Guid? SignedByUserId { get; init; }

    public DateTimeOffset? SignedAt { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }
}
