using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class VitalSignsRecord : AuditableEntity
{
    private const int MaxSourceLength = 100;
    private const int MaxNotesLength = 1000;

    private VitalSignsRecord()
    {
        SyncStatus = SyncStatus.Synced;
    }

    public VitalSignsRecord(
        Guid id,
        Guid organizationId,
        Guid patientId,
        Guid visitId,
        DateTimeOffset measuredAt,
        int? systolicBloodPressureMmHg = null,
        int? diastolicBloodPressureMmHg = null,
        int? heartRateBpm = null,
        int? respiratoryRatePerMinute = null,
        decimal? temperatureCelsius = null,
        int? oxygenSaturationPercent = null,
        decimal? weightKg = null,
        decimal? heightCm = null,
        decimal? glucoseMgDl = null,
        Guid? encounterId = null,
        Guid? measuredByUserId = null,
        string? source = null,
        string? notes = null,
        bool createdOffline = false,
        Guid? deviceId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        PatientId = RequireGuid(patientId, nameof(patientId));
        VisitId = RequireGuid(visitId, nameof(visitId));
        MeasuredAt = RequireMeasuredAt(measuredAt);
        EncounterId = encounterId;
        MeasuredByUserId = measuredByUserId;
        CreatedOffline = createdOffline;
        DeviceId = deviceId;
        SyncStatus = createdOffline ? SyncStatus.Pending : SyncStatus.Synced;

        UpdateMeasurements(
            systolicBloodPressureMmHg,
            diastolicBloodPressureMmHg,
            heartRateBpm,
            respiratoryRatePerMinute,
            temperatureCelsius,
            oxygenSaturationPercent,
            weightKg,
            heightCm,
            glucoseMgDl);

        UpdateContext(source, notes);
    }

    public Guid OrganizationId { get; private set; }

    public Guid PatientId { get; private set; }

    public Guid VisitId { get; private set; }

    public Guid? EncounterId { get; private set; }

    public Guid? MeasuredByUserId { get; private set; }

    public DateTimeOffset MeasuredAt { get; private set; }

    public int? SystolicBloodPressureMmHg { get; private set; }

    public int? DiastolicBloodPressureMmHg { get; private set; }

    public int? HeartRateBpm { get; private set; }

    public int? RespiratoryRatePerMinute { get; private set; }

    public decimal? TemperatureCelsius { get; private set; }

    public int? OxygenSaturationPercent { get; private set; }

    public decimal? WeightKg { get; private set; }

    public decimal? HeightCm { get; private set; }

    public decimal? GlucoseMgDl { get; private set; }

    public string? Source { get; private set; }

    public string? Notes { get; private set; }

    public bool CreatedOffline { get; private set; }

    public Guid? DeviceId { get; private set; }

    public SyncStatus SyncStatus { get; private set; }

    public void UpdateMeasuredAt(DateTimeOffset measuredAt)
    {
        MeasuredAt = RequireMeasuredAt(measuredAt);
    }

    public void AssignEncounter(Guid? encounterId)
    {
        EncounterId = encounterId;
    }

    public void AssignMeasuredByUser(Guid? measuredByUserId)
    {
        MeasuredByUserId = measuredByUserId;
    }

    public void UpdateContext(string? source, string? notes)
    {
        Source = NormalizeOptional(source, nameof(source), MaxSourceLength);
        Notes = NormalizeOptional(notes, nameof(notes), MaxNotesLength);
    }

    public void UpdateMeasurements(
        int? systolicBloodPressureMmHg,
        int? diastolicBloodPressureMmHg,
        int? heartRateBpm,
        int? respiratoryRatePerMinute,
        decimal? temperatureCelsius,
        int? oxygenSaturationPercent,
        decimal? weightKg,
        decimal? heightCm,
        decimal? glucoseMgDl)
    {
        EnsureAtLeastOneMeasurement(
            systolicBloodPressureMmHg,
            diastolicBloodPressureMmHg,
            heartRateBpm,
            respiratoryRatePerMinute,
            temperatureCelsius,
            oxygenSaturationPercent,
            weightKg,
            heightCm,
            glucoseMgDl);

        SystolicBloodPressureMmHg = RequirePositive(systolicBloodPressureMmHg, nameof(systolicBloodPressureMmHg));
        DiastolicBloodPressureMmHg = RequirePositive(diastolicBloodPressureMmHg, nameof(diastolicBloodPressureMmHg));
        HeartRateBpm = RequirePositive(heartRateBpm, nameof(heartRateBpm));
        RespiratoryRatePerMinute = RequirePositive(respiratoryRatePerMinute, nameof(respiratoryRatePerMinute));
        TemperatureCelsius = RequirePositive(temperatureCelsius, nameof(temperatureCelsius));
        OxygenSaturationPercent = RequirePercentage(oxygenSaturationPercent, nameof(oxygenSaturationPercent));
        WeightKg = RequirePositive(weightKg, nameof(weightKg));
        HeightCm = RequirePositive(heightCm, nameof(heightCm));
        GlucoseMgDl = RequirePositive(glucoseMgDl, nameof(glucoseMgDl));
    }

    public void UpdateSyncStatus(SyncStatus syncStatus)
    {
        SyncStatus = syncStatus;
    }

    private static DateTimeOffset RequireMeasuredAt(DateTimeOffset value)
    {
        if (value == default)
        {
            throw new DomainException("MeasuredAt is required.");
        }

        return value;
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static int? RequirePositive(int? value, string fieldName)
    {
        if (value.HasValue && value.Value <= 0)
        {
            throw new DomainException($"{fieldName} must be positive.");
        }

        return value;
    }

    private static decimal? RequirePositive(decimal? value, string fieldName)
    {
        if (value.HasValue && value.Value <= 0)
        {
            throw new DomainException($"{fieldName} must be positive.");
        }

        return value;
    }

    private static int? RequirePercentage(int? value, string fieldName)
    {
        if (!value.HasValue)
        {
            return null;
        }

        if (value.Value < 0 || value.Value > 100)
        {
            throw new DomainException($"{fieldName} must be between 0 and 100.");
        }

        return value;
    }

    private static void EnsureAtLeastOneMeasurement(
        int? systolicBloodPressureMmHg,
        int? diastolicBloodPressureMmHg,
        int? heartRateBpm,
        int? respiratoryRatePerMinute,
        decimal? temperatureCelsius,
        int? oxygenSaturationPercent,
        decimal? weightKg,
        decimal? heightCm,
        decimal? glucoseMgDl)
    {
        if (systolicBloodPressureMmHg.HasValue ||
            diastolicBloodPressureMmHg.HasValue ||
            heartRateBpm.HasValue ||
            respiratoryRatePerMinute.HasValue ||
            temperatureCelsius.HasValue ||
            oxygenSaturationPercent.HasValue ||
            weightKg.HasValue ||
            heightCm.HasValue ||
            glucoseMgDl.HasValue)
        {
            return;
        }

        throw new DomainException("At least one vital signs measurement is required.");
    }

    private static string? NormalizeOptional(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }
}