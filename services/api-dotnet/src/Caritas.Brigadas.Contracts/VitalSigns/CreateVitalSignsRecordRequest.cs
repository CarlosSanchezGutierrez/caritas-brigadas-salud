namespace Caritas.Brigadas.Contracts.VitalSigns;

public sealed record CreateVitalSignsRecordRequest
{
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

    public Guid? DeviceId { get; init; }
}