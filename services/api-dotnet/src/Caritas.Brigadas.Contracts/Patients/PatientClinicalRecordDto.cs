namespace Caritas.Brigadas.Contracts.Patients;

public sealed record PatientClinicalRecordDto
{
    public Guid OrganizationId { get; init; }

    public Guid PatientId { get; init; }

    public PatientSummaryDto Patient { get; init; } = new();

    public IReadOnlyCollection<PatientClinicalRecordVisitDto> Visits { get; init; } = Array.Empty<PatientClinicalRecordVisitDto>();

    public IReadOnlyCollection<PatientClinicalRecordEncounterDto> Encounters { get; init; } = Array.Empty<PatientClinicalRecordEncounterDto>();

    public IReadOnlyCollection<PatientClinicalRecordVitalSignsDto> VitalSigns { get; init; } = Array.Empty<PatientClinicalRecordVitalSignsDto>();

    public IReadOnlyCollection<PatientClinicalRecordFormResponseDto> FormResponses { get; init; } = Array.Empty<PatientClinicalRecordFormResponseDto>();

    public IReadOnlyCollection<PatientClinicalRecordConsentDocumentDto> ConsentDocuments { get; init; } = Array.Empty<PatientClinicalRecordConsentDocumentDto>();

    public IReadOnlyCollection<PatientClinicalRecordMedicalReferralDto> MedicalReferrals { get; init; } = Array.Empty<PatientClinicalRecordMedicalReferralDto>();

    public IReadOnlyCollection<PatientClinicalRecordMedicationDeliveryDto> MedicationDeliveries { get; init; } = Array.Empty<PatientClinicalRecordMedicationDeliveryDto>();

    public PatientClinicalRecordSummaryDto Summary { get; init; } = new();
}

public sealed record PatientClinicalRecordSummaryDto
{
    public int VisitCount { get; init; }

    public int EncounterCount { get; init; }

    public int VitalSignsCount { get; init; }

    public int FormResponseCount { get; init; }

    public int ConsentDocumentCount { get; init; }

    public int MedicalReferralCount { get; init; }

    public int MedicationDeliveryCount { get; init; }

    public DateTimeOffset? FirstVisitAt { get; init; }

    public DateTimeOffset? LastVisitAt { get; init; }

    public DateTimeOffset? LastVitalSignsMeasuredAt { get; init; }
}

public sealed record PatientClinicalRecordVisitDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string VisitFolio { get; init; } = string.Empty;

    public Guid PatientId { get; init; }

    public Guid BrigadeId { get; init; }

    public DateTimeOffset? ArrivalTime { get; init; }

    public Guid? RegisteredByUserId { get; init; }

    public string VisitStatus { get; init; } = string.Empty;

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public string SyncStatus { get; init; } = string.Empty;

    public DateTimeOffset? ClosedAt { get; init; }

    public Guid? ClosedByUserId { get; init; }

    public bool IsActive { get; init; }

    public bool IsClosed { get; init; }

    public bool NeedsReview { get; init; }
}

public sealed record PatientClinicalRecordEncounterDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string EncounterFolio { get; init; } = string.Empty;

    public Guid VisitId { get; init; }

    public Guid PatientId { get; init; }

    public Guid BrigadeId { get; init; }

    public Guid ServiceId { get; init; }

    public Guid? ProviderUserId { get; init; }

    public DateTimeOffset? StartedAt { get; init; }

    public DateTimeOffset? CompletedAt { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public string SyncStatus { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public bool IsCompleted { get; init; }

    public bool NeedsReview { get; init; }
}

public sealed record PatientClinicalRecordVitalSignsDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid PatientId { get; init; }

    public Guid VisitId { get; init; }

    public Guid? EncounterId { get; init; }

    public Guid? MeasuredByUserId { get; init; }

    public DateTimeOffset MeasuredAt { get; init; }

    public int? SystolicBloodPressureMmHg { get; init; }

    public int? DiastolicBloodPressureMmHg { get; init; }

    public int? HeartRateBpm { get; init; }

    public int? RespiratoryRatePerMinute { get; init; }

    public decimal? TemperatureCelsius { get; init; }

    public int? OxygenSaturationPercent { get; init; }

    public decimal? WeightKg { get; init; }

    public decimal? HeightCm { get; init; }

    public decimal? GlucoseMgDl { get; init; }

    public string? Source { get; init; }

    public string? Notes { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public string SyncStatus { get; init; } = string.Empty;
}

public sealed record PatientClinicalRecordFormResponseDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid EncounterId { get; init; }

    public Guid FormTemplateId { get; init; }

    public string? ResponseHash { get; init; }

    public Guid? CompletedByUserId { get; init; }

    public DateTimeOffset? CompletedAt { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public DateTimeOffset? SubmittedAt { get; init; }

    public DateTimeOffset? CapturedAt { get; init; }

    public string SyncStatus { get; init; } = string.Empty;
}

public sealed record PatientClinicalRecordConsentDocumentDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid PatientId { get; init; }

    public Guid? VisitId { get; init; }

    public string ConsentType { get; init; } = string.Empty;

    public string DocumentVersion { get; init; } = string.Empty;

    public bool HasSignature { get; init; }

    public string? GuardianFullName { get; init; }

    public string? GuardianRelationship { get; init; }

    public Guid? SignedByUserId { get; init; }

    public DateTimeOffset SignedAt { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }

    public string SyncStatus { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record PatientClinicalRecordMedicalReferralDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid EncounterId { get; init; }

    public Guid PatientId { get; init; }

    public string ReferralFolio { get; init; } = string.Empty;

    public string? DestinationInstitution { get; init; }

    public string ReferralReason { get; init; } = string.Empty;

    public string? Priority { get; init; }

    public Guid? ReferredByUserId { get; init; }

    public Guid? ProviderSignatureId { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record PatientClinicalRecordMedicationDeliveryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid EncounterId { get; init; }

    public Guid PatientId { get; init; }

    public string MedicationName { get; init; } = string.Empty;

    public string? Presentation { get; init; }

    public string? Quantity { get; init; }

    public string? LotNumber { get; init; }

    public DateOnly? ExpirationDate { get; init; }

    public string? Instructions { get; init; }

    public Guid? DeliveredByUserId { get; init; }

    public string? ReceivedByName { get; init; }

    public Guid? SignatureId { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}