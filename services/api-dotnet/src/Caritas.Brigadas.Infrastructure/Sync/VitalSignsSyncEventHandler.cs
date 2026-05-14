using System.Text.Json;
using Caritas.Brigadas.Contracts.VitalSigns;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class VitalSignsSyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public VitalSignsSyncEventHandler(
        CaritasDbContext dbContext,
        JsonSerializerOptions payloadJsonOptions)
    {
        _dbContext = dbContext;
        _payloadJsonOptions = payloadJsonOptions;
    }

    public async Task HandleAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<Guid> acceptedVitalSignsIdsInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "vital_signs_operation_not_implemented");

            return;
        }

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Vital signs",
                _payloadJsonOptions,
                out CreateVitalSignsRecordRequest? request,
                out var payloadRejectionReason))
        {
            syncEvent.Reject(
                processedAt,
                payloadRejectionReason);

            return;
        }

        if (request.PatientId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "PatientId is required for vital signs sync.");

            return;
        }

        if (request.VisitId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "VisitId is required for vital signs sync.");

            return;
        }

        var patientExists =
            _dbContext.Patients.Local.Any(patient =>
                patient.Id == request.PatientId &&
                patient.OrganizationId == organizationId &&
                !patient.IsDeleted) ||
            await _dbContext.Patients
                .AsNoTracking()
                .AnyAsync(
                    patient =>
                        patient.Id == request.PatientId &&
                        patient.OrganizationId == organizationId &&
                        !patient.IsDeleted,
                    cancellationToken);

        if (!patientExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "vital_signs_patient_not_found");

            return;
        }

        var visitExists =
            _dbContext.PatientVisits.Local.Any(visit =>
                visit.Id == request.VisitId &&
                visit.OrganizationId == organizationId &&
                visit.PatientId == request.PatientId &&
                visit.BrigadeId == batch.BrigadeId &&
                !visit.IsDeleted) ||
            await _dbContext.PatientVisits
                .AsNoTracking()
                .AnyAsync(
                    visit =>
                        visit.Id == request.VisitId &&
                        visit.OrganizationId == organizationId &&
                        visit.PatientId == request.PatientId &&
                        visit.BrigadeId == batch.BrigadeId &&
                        !visit.IsDeleted,
                    cancellationToken);

        if (!visitExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "vital_signs_visit_not_found");

            return;
        }

        if (request.EncounterId.HasValue)
        {
            var encounterExists =
                _dbContext.ServiceEncounters.Local.Any(encounter =>
                    encounter.Id == request.EncounterId.Value &&
                    encounter.OrganizationId == organizationId &&
                    encounter.PatientId == request.PatientId &&
                    encounter.VisitId == request.VisitId &&
                    !encounter.IsDeleted) ||
                await _dbContext.ServiceEncounters
                    .AsNoTracking()
                    .AnyAsync(
                        encounter =>
                            encounter.Id == request.EncounterId.Value &&
                            encounter.OrganizationId == organizationId &&
                            encounter.PatientId == request.PatientId &&
                            encounter.VisitId == request.VisitId &&
                            !encounter.IsDeleted,
                        cancellationToken);

            if (!encounterExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "vital_signs_encounter_not_found");

                return;
            }
        }

        if (request.MeasuredByUserId.HasValue)
        {
            var measuredByUserExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.MeasuredByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!measuredByUserExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "vital_signs_measured_by_user_not_found");

                return;
            }
        }

        var vitalSignsRecordId = syncEvent.EntityId ?? Guid.NewGuid();

        var vitalSignsIdAlreadyExists =
            acceptedVitalSignsIdsInBatch.Contains(vitalSignsRecordId) ||
            _dbContext.VitalSignsRecords.Local.Any(record =>
                record.Id == vitalSignsRecordId &&
                record.OrganizationId == organizationId &&
                !record.IsDeleted) ||
            await _dbContext.VitalSignsRecords
                .AsNoTracking()
                .AnyAsync(
                    record =>
                        record.Id == vitalSignsRecordId &&
                        record.OrganizationId == organizationId &&
                        !record.IsDeleted,
                    cancellationToken);

        if (vitalSignsIdAlreadyExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "vital_signs_id_already_exists");

            return;
        }

        try
        {
            if (!acceptedVitalSignsIdsInBatch.Add(vitalSignsRecordId))
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "vital_signs_duplicate_in_pending_batch");

                return;
            }

            var vitalSignsRecord = new VitalSignsRecord(
                vitalSignsRecordId,
                organizationId,
                request.PatientId,
                request.VisitId,
                request.MeasuredAt,
                request.SystolicBloodPressureMmHg,
                request.DiastolicBloodPressureMmHg,
                request.HeartRateBpm,
                request.RespiratoryRatePerMinute,
                request.TemperatureCelsius,
                request.OxygenSaturationPercent,
                request.WeightKg,
                request.HeightCm,
                request.GlucoseMgDl,
                request.EncounterId,
                request.MeasuredByUserId,
                request.Source,
                request.Notes,
                createdOffline: true,
                deviceId: request.DeviceId ?? batch.DeviceId);

            _dbContext.VitalSignsRecords.Add(vitalSignsRecord);

            syncEvent.Accept(
                processedAt,
                vitalSignsRecord.Id);
        }
        catch (DomainException exception)
        {
            syncEvent.Reject(
                processedAt,
                exception.Message);
        }
    }
}
