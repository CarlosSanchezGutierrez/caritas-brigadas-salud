using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Contracts.Sync;
using Caritas.Brigadas.Contracts.VitalSigns;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

public sealed class SyncBatchProcessor : ISyncBatchProcessor
{
    private const string SkeletonConflictReason = "sync_processor_domain_handler_not_implemented";
    private static readonly JsonSerializerOptions PayloadJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CaritasDbContext _dbContext;

    public SyncBatchProcessor(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProcessSyncBatchResultDto> ProcessAsync(
        Guid organizationId,
        Guid syncBatchId,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new DomainException("Organization id is required.");
        }

        if (syncBatchId == Guid.Empty)
        {
            throw new DomainException("Sync batch id is required.");
        }

        var batch = await _dbContext.SyncBatches
            .SingleOrDefaultAsync(
                item =>
                    item.Id == syncBatchId &&
                    item.OrganizationId == organizationId,
                cancellationToken);

        if (batch is null)
        {
            throw new KeyNotFoundException("Sync batch was not found.");
        }

        if (batch.IsCompleted)
        {
            return new ProcessSyncBatchResultDto
            {
                Batch = ToSummary(batch),
                PendingEventsProcessed = 0,
                AcceptedCount = batch.AcceptedCount,
                RejectedCount = batch.RejectedCount,
                ConflictCount = batch.ConflictCount,
                Completed = true,
                Message = "Sync batch was already completed."
            };
        }

        if (batch.Status == SyncBatchStatus.Failed)
        {
            throw new InvalidOperationException("Failed sync batch cannot be processed.");
        }

        if (batch.Status == SyncBatchStatus.Received)
        {
            batch.MarkProcessing();
        }

        var pendingEvents = await _dbContext.SyncEvents
            .Where(syncEvent =>
                syncEvent.OrganizationId == organizationId &&
                syncEvent.SyncBatchId == syncBatchId &&
                syncEvent.Status == SyncEventStatus.Pending)
            .OrderBy(syncEvent => syncEvent.ReceivedAtServer)
            .ThenBy(syncEvent => syncEvent.Id)
            .ToArrayAsync(cancellationToken);

        
        pendingEvents = pendingEvents
            .OrderBy(GetSyncProcessingOrder)
            .ThenBy(syncEvent => syncEvent.ReceivedAtServer)
            .ThenBy(syncEvent => syncEvent.Id)
            .ToArray();
var processedAt = DateTimeOffset.UtcNow;
        var processedCount = 0;
        var acceptedPatientFoliosInBatch = new HashSet<string>(StringComparer.Ordinal);
        var acceptedVisitFoliosInBatch = new HashSet<string>(StringComparer.Ordinal);
        var acceptedVitalSignsIdsInBatch = new HashSet<Guid>();

        foreach (var syncEvent in pendingEvents)
        {
            syncEvent.MarkProcessing();

            if (!TryValidateEvent(syncEvent, out var rejectionReason))
            {
                syncEvent.Reject(
                    processedAt,
                    rejectionReason ?? "Sync event payload is invalid.");

                processedCount++;
                continue;
            }

            if (syncEvent.EntityType == SyncEntityType.Patient)
            {
                await HandlePatientEventAsync(
                    organizationId,
                    syncEvent,
                    processedAt,
                    acceptedPatientFoliosInBatch,
                    cancellationToken);

                processedCount++;
                continue;
            }

            if (syncEvent.EntityType == SyncEntityType.PatientVisit)
            {
                await HandlePatientVisitEventAsync(
                    organizationId,
                    batch,
                    syncEvent,
                    processedAt,
                    acceptedVisitFoliosInBatch,
                    cancellationToken);

                processedCount++;
                continue;
            }

            if (syncEvent.EntityType == SyncEntityType.VitalSigns)
            {
                await HandleVitalSignsEventAsync(
                    organizationId,
                    batch,
                    syncEvent,
                    processedAt,
                    acceptedVitalSignsIdsInBatch,
                    cancellationToken);

                processedCount++;
                continue;
            }
            syncEvent.MarkConflict(
                processedAt,
                SkeletonConflictReason);

            processedCount++;
        }

        var allEvents = await _dbContext.SyncEvents
            .Where(syncEvent =>
                syncEvent.OrganizationId == organizationId &&
                syncEvent.SyncBatchId == syncBatchId)
            .ToArrayAsync(cancellationToken);

        var acceptedCount = allEvents.Count(syncEvent => syncEvent.Status == SyncEventStatus.Accepted);
        var rejectedCount = allEvents.Count(syncEvent => syncEvent.Status == SyncEventStatus.Rejected);
        var conflictCount = allEvents.Count(syncEvent => syncEvent.Status == SyncEventStatus.Conflict);

        batch.Complete(
            DateTimeOffset.UtcNow,
            acceptedCount,
            rejectedCount,
            conflictCount,
            conflictCount > 0
                ? "Sync processor marked unsupported or conflicting events as conflicts."
                : null);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProcessSyncBatchResultDto
        {
            Batch = ToSummary(batch),
            PendingEventsProcessed = processedCount,
            AcceptedCount = acceptedCount,
            RejectedCount = rejectedCount,
            ConflictCount = conflictCount,
            Completed = batch.IsCompleted,
            Message = "Sync batch processor completed patient, visit, and vital signs handler processing."
        };
    }

    private static int GetSyncProcessingOrder(SyncEvent syncEvent)
    {
        if (syncEvent.EntityType == SyncEntityType.Patient &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 0;
        }

        if (syncEvent.EntityType == SyncEntityType.PatientVisit &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 1;
        }

        return 2;
    }

    private async Task HandlePatientEventAsync(
        Guid organizationId,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<string> acceptedPatientFoliosInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_operation_not_implemented");

            return;
        }

        CreatePatientRequest? request;

        try
        {
            using var document = JsonDocument.Parse(syncEvent.PayloadJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                syncEvent.Reject(
                    processedAt,
                    "Patient payload must be a JSON object.");

                return;
            }

            request = JsonSerializer.Deserialize<CreatePatientRequest>(
                syncEvent.PayloadJson,
                PayloadJsonOptions);
        }
        catch (JsonException)
        {
            syncEvent.Reject(
                processedAt,
                "Patient payload JSON is invalid.");

            return;
        }

        if (request is null)
        {
            syncEvent.Reject(
                processedAt,
                "Patient payload is required.");

            return;
        }

        var patientId = syncEvent.EntityId ?? Guid.NewGuid();

        var patientIdAlreadyExists = await _dbContext.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.Id == patientId &&
                    patient.OrganizationId == organizationId &&
                    !patient.IsDeleted,
                cancellationToken);

        if (patientIdAlreadyExists ||
            _dbContext.Patients.Local.Any(patient => patient.Id == patientId && patient.OrganizationId == organizationId && !patient.IsDeleted))
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_id_already_exists");

            return;
        }

        var patientFolio = string.IsNullOrWhiteSpace(request.PatientFolio)
            ? GenerateSyncPatientFolio(syncEvent)
            : request.PatientFolio.Trim();

        var normalizedFolio = patientFolio.ToUpperInvariant();

        if (acceptedPatientFoliosInBatch.Contains(normalizedFolio))
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_folio_duplicate_in_pending_batch");

            return;
        }

        var folioExists = await _dbContext.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.OrganizationId == organizationId &&
                    patient.PatientFolio == normalizedFolio &&
                    !patient.IsDeleted,
                cancellationToken);

        if (folioExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_folio_already_exists");

            return;
        }

        try
        {
            var patient = new Patient(
                patientId,
                organizationId,
                patientFolio,
                request.FirstName,
                request.PaternalLastName,
                request.MaternalLastName,
                request.BirthDate,
                request.ApproximateAge,
                ParseSex(request.Sex));

            patient.UpdateSensitiveIdentifiers(
                request.Curp,
                request.Phone);

            patient.UpdateLocation(
                request.AddressLine,
                request.Municipality,
                request.Colony,
                request.Community);

            if (request.IsMigrant)
            {
                patient.MarkAsMigrant();
            }

            if (request.IsPartialRecord)
            {
                if (string.IsNullOrWhiteSpace(request.PartialRecordReason))
                {
                    syncEvent.Reject(
                        processedAt,
                        "Partial record reason is required when patient record is marked as partial.");

                    return;
                }

                patient.MarkAsPartialRecord(request.PartialRecordReason);
            }

            patient.UpdateAdminNotes(request.NotesAdmin);

            if (!acceptedPatientFoliosInBatch.Add(normalizedFolio))
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "patient_folio_duplicate_in_pending_batch");

                return;
            }

            _dbContext.Patients.Add(patient);

            syncEvent.Accept(
                processedAt,
                patient.Id);
        }
        catch (DomainException exception)
        {
            syncEvent.Reject(
                processedAt,
                exception.Message);
        }
    }

    private async Task HandlePatientVisitEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<string> acceptedVisitFoliosInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_visit_operation_not_implemented");

            return;
        }

        CreatePatientVisitRequest? request;

        try
        {
            using var document = JsonDocument.Parse(syncEvent.PayloadJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                syncEvent.Reject(
                    processedAt,
                    "Patient visit payload must be a JSON object.");

                return;
            }

            request = JsonSerializer.Deserialize<CreatePatientVisitRequest>(
                syncEvent.PayloadJson,
                PayloadJsonOptions);
        }
        catch (JsonException)
        {
            syncEvent.Reject(
                processedAt,
                "Patient visit payload JSON is invalid.");

            return;
        }

        if (request is null)
        {
            syncEvent.Reject(
                processedAt,
                "Patient visit payload is required.");

            return;
        }

        if (request.PatientId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "PatientId is required for patient visit sync.");

            return;
        }

        if (request.BrigadeId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "BrigadeId is required for patient visit sync.");

            return;
        }

        if (request.BrigadeId != batch.BrigadeId)
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_visit_brigade_mismatch");

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
                "patient_visit_patient_not_found");

            return;
        }

        var brigadeExists = await _dbContext.Brigades
            .AsNoTracking()
            .AnyAsync(
                brigade =>
                    brigade.Id == request.BrigadeId &&
                    brigade.OrganizationId == organizationId &&
                    !brigade.IsDeleted,
                cancellationToken);

        if (!brigadeExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_visit_brigade_not_found");

            return;
        }

        if (request.RegisteredByUserId.HasValue)
        {
            var userExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.RegisteredByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!userExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "patient_visit_registered_by_user_not_found");

                return;
            }
        }

        var visitId = syncEvent.EntityId ?? Guid.NewGuid();

        var visitIdAlreadyExists =
            _dbContext.PatientVisits.Local.Any(visit =>
                visit.Id == visitId &&
                visit.OrganizationId == organizationId &&
                !visit.IsDeleted) ||
            await _dbContext.PatientVisits
                .AsNoTracking()
                .AnyAsync(
                    visit =>
                        visit.Id == visitId &&
                        visit.OrganizationId == organizationId &&
                        !visit.IsDeleted,
                    cancellationToken);

        if (visitIdAlreadyExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_visit_id_already_exists");

            return;
        }

        var visitFolio = string.IsNullOrWhiteSpace(request.VisitFolio)
            ? GenerateSyncVisitFolio(syncEvent)
            : request.VisitFolio.Trim();

        var normalizedVisitFolio = visitFolio.ToUpperInvariant();

        if (acceptedVisitFoliosInBatch.Contains(normalizedVisitFolio))
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_visit_folio_duplicate_in_pending_batch");

            return;
        }

        var visitFolioExists = await _dbContext.PatientVisits
            .AsNoTracking()
            .AnyAsync(
                visit =>
                    visit.OrganizationId == organizationId &&
                    visit.VisitFolio == normalizedVisitFolio &&
                    !visit.IsDeleted,
                cancellationToken);

        if (visitFolioExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "patient_visit_folio_already_exists");

            return;
        }

        try
        {
            var visit = new PatientVisit(
                visitId,
                organizationId,
                visitFolio,
                request.PatientId,
                request.BrigadeId,
                request.ArrivalTime,
                request.RegisteredByUserId,
                createdOffline: true,
                deviceId: request.DeviceId ?? batch.DeviceId);

            if (!acceptedVisitFoliosInBatch.Add(normalizedVisitFolio))
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "patient_visit_folio_duplicate_in_pending_batch");

                return;
            }

            _dbContext.PatientVisits.Add(visit);

            syncEvent.Accept(
                processedAt,
                visit.Id);
        }
        catch (DomainException exception)
        {
            syncEvent.Reject(
                processedAt,
                exception.Message);
        }
    }

    private async Task HandleVitalSignsEventAsync(
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

        CreateVitalSignsRecordRequest? request;

        try
        {
            using var document = JsonDocument.Parse(syncEvent.PayloadJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                syncEvent.Reject(
                    processedAt,
                    "Vital signs payload must be a JSON object.");

                return;
            }

            request = JsonSerializer.Deserialize<CreateVitalSignsRecordRequest>(
                syncEvent.PayloadJson,
                PayloadJsonOptions);
        }
        catch (JsonException)
        {
            syncEvent.Reject(
                processedAt,
                "Vital signs payload JSON is invalid.");

            return;
        }

        if (request is null)
        {
            syncEvent.Reject(
                processedAt,
                "Vital signs payload is required.");

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
    private static bool TryValidateEvent(
        SyncEvent syncEvent,
        out string? rejectionReason)
    {
        rejectionReason = null;

        if (!SyncEntityType.IsAllowed(syncEvent.EntityType))
        {
            rejectionReason = "Sync event entity type is not allowed.";
            return false;
        }

        if (!SyncOperation.IsAllowed(syncEvent.Operation))
        {
            rejectionReason = "Sync event operation is not allowed.";
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(syncEvent.PayloadJson);

            if (document.RootElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                rejectionReason = "Sync event payload JSON root is empty.";
                return false;
            }
        }
        catch (JsonException)
        {
            rejectionReason = "Sync event payload JSON is invalid.";
            return false;
        }

        return true;
    }

    private static string GenerateSyncPatientFolio(SyncEvent syncEvent)
    {
        return $"PAT-SYNC-{syncEvent.Id:N}"[..41];
    }

    private static string GenerateSyncVisitFolio(SyncEvent syncEvent)
    {
        return $"VIS-SYNC-{syncEvent.Id:N}"[..41];
    }

    private static Sex ParseSex(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Sex.NotSpecified;
        }

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "male" or "masculino" or "m" => Sex.Male,
            "female" or "femenino" or "f" => Sex.Female,
            _ => Sex.NotSpecified
        };
    }

    private static SyncBatchSummaryDto ToSummary(SyncBatch batch)
    {
        return new SyncBatchSummaryDto
        {
            Id = batch.Id,
            OrganizationId = batch.OrganizationId,
            UserId = batch.UserId,
            BrigadeId = batch.BrigadeId,
            DeviceId = batch.DeviceId,
            EventsCount = batch.EventsCount,
            Status = batch.Status,
            StartedAt = batch.StartedAt,
            CompletedAt = batch.CompletedAt,
            ErrorSummary = batch.ErrorSummary,
            IsCompleted = batch.IsCompleted
        };
    }
}