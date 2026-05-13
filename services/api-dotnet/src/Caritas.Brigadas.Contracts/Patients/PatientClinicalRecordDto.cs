namespace Caritas.Brigadas.Contracts.Patients;

public sealed record PatientClinicalRecordDto
{
    public Guid OrganizationId { get; init; }

    public Guid PatientId { get; init; }

    public PatientSummaryDto Patient { get; init; } = new();

    public IReadOnlyCollection<PatientClinicalRecordVisitDto> Visits { get; init; } = Array.Empty<PatientClinicalRecordVisitDto>();

    public IReadOnlyCollection<PatientClinicalRecordEncounterDto> Encounters { get; init; } = Array.Empty<PatientClinicalRecordEncounterDto>();

    public IReadOnlyCollection<PatientClinicalRecordVitalSignsDto> VitalSigns { get; init; } = Array.Empty<PatientClinicalRecordVitalSignsDto>();

    public PatientClinicalRecordSummaryDto Summary { get; init; } = new();
}

public sealed record PatientClinicalRecordSummaryDto
{
    public int VisitCount { get; init; }

    public int EncounterCount { get; init; }

    public int VitalSignsCount { get; init; }

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