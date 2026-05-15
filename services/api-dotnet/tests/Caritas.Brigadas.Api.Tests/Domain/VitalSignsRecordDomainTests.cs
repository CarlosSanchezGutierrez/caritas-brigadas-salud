using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Domain;

public sealed class VitalSignsRecordDomainTests
{
    [Fact]
    public void Constructor_CreatesHistoricalTenantScopedVitalSignsRecord()
    {
        var organizationId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var visitId = Guid.NewGuid();
        var encounterId = Guid.NewGuid();
        var measuredByUserId = Guid.NewGuid();
        var measuredAt = DateTimeOffset.UtcNow;

        var record = new VitalSignsRecord(
            Guid.NewGuid(),
            organizationId,
            patientId,
            visitId,
            measuredAt,
            systolicBloodPressureMmHg: 120,
            diastolicBloodPressureMmHg: 80,
            heartRateBpm: 72,
            respiratoryRatePerMinute: 16,
            temperatureCelsius: 36.6m,
            oxygenSaturationPercent: 98,
            weightKg: 70.5m,
            heightCm: 175.2m,
            glucoseMgDl: 90m,
            encounterId: encounterId,
            measuredByUserId: measuredByUserId,
            source: "office-capture",
            notes: "Initial capture.");

        Assert.Equal(organizationId, record.OrganizationId);
        Assert.Equal(patientId, record.PatientId);
        Assert.Equal(visitId, record.VisitId);
        Assert.Equal(encounterId, record.EncounterId);
        Assert.Equal(measuredByUserId, record.MeasuredByUserId);
        Assert.Equal(measuredAt, record.MeasuredAt);
        Assert.Equal(120, record.SystolicBloodPressureMmHg);
        Assert.Equal(80, record.DiastolicBloodPressureMmHg);
        Assert.Equal(72, record.HeartRateBpm);
        Assert.Equal(16, record.RespiratoryRatePerMinute);
        Assert.Equal(36.6m, record.TemperatureCelsius);
        Assert.Equal(98, record.OxygenSaturationPercent);
        Assert.Equal(70.5m, record.WeightKg);
        Assert.Equal(175.2m, record.HeightCm);
        Assert.Equal(90m, record.GlucoseMgDl);
        Assert.Equal("office-capture", record.Source);
        Assert.Equal("Initial capture.", record.Notes);
        Assert.Equal(SyncStatus.Synced, record.SyncStatus);
    }

    [Fact]
    public void Constructor_MarksOfflineRecordAsPendingSync()
    {
        var record = new VitalSignsRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            heartRateBpm: 72,
            createdOffline: true,
            deviceId: Guid.NewGuid());

        Assert.True(record.CreatedOffline);
        Assert.Equal(SyncStatus.Pending, record.SyncStatus);
        Assert.NotNull(record.DeviceId);
    }

    [Fact]
    public void Constructor_RequiresAtLeastOneMeasurement()
    {
        Assert.Throws<DomainException>(() =>
            new VitalSignsRecord(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsInvalidPositiveIntegerMeasurements(int invalidValue)
    {
        Assert.Throws<DomainException>(() =>
            new VitalSignsRecord(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                heartRateBpm: invalidValue));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_RejectsInvalidOxygenSaturation(int invalidValue)
    {
        Assert.Throws<DomainException>(() =>
            new VitalSignsRecord(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                oxygenSaturationPercent: invalidValue));
    }

    [Fact]
    public void Constructor_RejectsEmptyTenantPatientOrVisit()
    {
        Assert.Throws<DomainException>(() =>
            new VitalSignsRecord(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                heartRateBpm: 70));

        Assert.Throws<DomainException>(() =>
            new VitalSignsRecord(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                heartRateBpm: 70));

        Assert.Throws<DomainException>(() =>
            new VitalSignsRecord(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                DateTimeOffset.UtcNow,
                heartRateBpm: 70));
    }

    [Fact]
    public void UpdateMeasurements_PreservesHistoricalRecordAndValidatesValues()
    {
        var record = new VitalSignsRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            heartRateBpm: 70);

        record.UpdateMeasurements(
            systolicBloodPressureMmHg: 118,
            diastolicBloodPressureMmHg: 78,
            heartRateBpm: 74,
            respiratoryRatePerMinute: 18,
            temperatureCelsius: 36.7m,
            oxygenSaturationPercent: 97,
            weightKg: 71m,
            heightCm: 175m,
            glucoseMgDl: 95m);

        Assert.Equal(118, record.SystolicBloodPressureMmHg);
        Assert.Equal(78, record.DiastolicBloodPressureMmHg);
        Assert.Equal(74, record.HeartRateBpm);
        Assert.Equal(18, record.RespiratoryRatePerMinute);
        Assert.Equal(36.7m, record.TemperatureCelsius);
        Assert.Equal(97, record.OxygenSaturationPercent);
        Assert.Equal(71m, record.WeightKg);
        Assert.Equal(175m, record.HeightCm);
        Assert.Equal(95m, record.GlucoseMgDl);
    }
}