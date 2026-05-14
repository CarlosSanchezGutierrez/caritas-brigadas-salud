using System.Reflection;
using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Contracts.ServiceEncounters;
using Caritas.Brigadas.Contracts.FormResponses;
using Caritas.Brigadas.Contracts.ConsentDocuments;
using Caritas.Brigadas.Contracts.MedicalReferrals;
using Caritas.Brigadas.Contracts.MedicationDeliveries;
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
    private readonly PatientSyncEventHandler _patientSyncEventHandler;

    public SyncBatchProcessor(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
        _patientSyncEventHandler = new PatientSyncEventHandler(dbContext, PayloadJsonOptions);
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
            .OrderBy(SyncProcessingOrder.GetOrder)
            .ThenBy(syncEvent => syncEvent.ReceivedAtServer)
            .ThenBy(syncEvent => syncEvent.Id)
            .ToArray();

        var processedAt = DateTimeOffset.UtcNow;
        var processedCount = 0;
        var reservationState = new PendingBatchReservationState();

        foreach (var syncEvent in pendingEvents)
        {
            await ProcessPendingEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState,
                cancellationToken);

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
            Message = "Sync batch processor completed patient, visit, service encounter, vital signs, form response, consent document, medical referral, and medication delivery handler processing."
        };
    }

    private async Task ProcessPendingEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        PendingBatchReservationState reservationState,
        CancellationToken cancellationToken)
    {
        syncEvent.MarkProcessing();

        if (!TryValidateEvent(syncEvent, out var rejectionReason))
        {
            syncEvent.Reject(
                processedAt,
                rejectionReason ?? "Sync event payload is invalid.");

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.Patient)
        {
            await HandlePatientEventAsync(
                organizationId,
                syncEvent,
                processedAt,
                reservationState.AcceptedPatientFoliosInBatch,
                cancellationToken);

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.PatientVisit)
        {
            await HandlePatientVisitEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState.AcceptedVisitFoliosInBatch,
                cancellationToken);

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.ServiceEncounter)
        {
            await HandleServiceEncounterEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState.AcceptedEncounterFoliosInBatch,
                reservationState.AcceptedEncounterVisitServiceKeysInBatch,
                cancellationToken);

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.VitalSigns)
        {
            await HandleVitalSignsEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState.AcceptedVitalSignsIdsInBatch,
                cancellationToken);

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.FormResponse)
        {
            await HandleFormResponseEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState.AcceptedFormResponseIdsInBatch,
                reservationState.AcceptedFormResponseEncounterTemplateKeysInBatch,
                cancellationToken);

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.ConsentDocument)
        {
            await HandleConsentDocumentEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState.AcceptedConsentDocumentIdsInBatch,
                reservationState.AcceptedConsentDocumentKeysInBatch,
                cancellationToken);

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.MedicalReferral)
        {
            await HandleMedicalReferralEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState.AcceptedMedicalReferralIdsInBatch,
                reservationState.AcceptedMedicalReferralFoliosInBatch,
                cancellationToken);

            return;
        }

        if (syncEvent.EntityType == SyncEntityType.MedicationDelivery)
        {
            await HandleMedicationDeliveryEventAsync(
                organizationId,
                batch,
                syncEvent,
                processedAt,
                reservationState.AcceptedMedicationDeliveryIdsInBatch,
                cancellationToken);

            return;
        }

        syncEvent.MarkConflict(
            processedAt,
            SkeletonConflictReason);
    }
    private static int GetSyncProcessingOrder(SyncEvent syncEvent)
    {
        return SyncProcessingOrder.GetOrder(syncEvent);
    }

    private async Task HandlePatientEventAsync(
        Guid organizationId,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<string> acceptedPatientFoliosInBatch,
        CancellationToken cancellationToken)
    {
        await _patientSyncEventHandler.HandleAsync(
            organizationId,
            syncEvent,
            processedAt,
            acceptedPatientFoliosInBatch,
            cancellationToken);
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

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Patient visit",
                PayloadJsonOptions,
                out CreatePatientVisitRequest? request,
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

    private async Task HandleServiceEncounterEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<string> acceptedEncounterFoliosInBatch,
        ISet<string> acceptedEncounterVisitServiceKeysInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_operation_not_implemented");

            return;
        }

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Service encounter",
                PayloadJsonOptions,
                out CreateServiceEncounterRequest? request,
                out var payloadRejectionReason))
        {
            syncEvent.Reject(
                processedAt,
                payloadRejectionReason);

            return;
        }

        if (request.VisitId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "VisitId is required for service encounter sync.");

            return;
        }

        if (string.IsNullOrWhiteSpace(request.ServiceCode))
        {
            syncEvent.Reject(
                processedAt,
                "ServiceCode is required for service encounter sync.");

            return;
        }

        PatientVisit? trackedVisit = _dbContext.PatientVisits.Local.FirstOrDefault(visit =>
            visit.Id == request.VisitId &&
            visit.OrganizationId == organizationId &&
            !visit.IsDeleted);

        var visit = trackedVisit ?? await _dbContext.PatientVisits
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.VisitId &&
                    item.OrganizationId == organizationId &&
                    !item.IsDeleted,
                cancellationToken);

        if (visit is null)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_visit_not_found");

            return;
        }

        if (visit.BrigadeId != batch.BrigadeId)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_brigade_mismatch");

            return;
        }

        if (visit.IsClosed)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_visit_closed");

            return;
        }

        var serviceCode = request.ServiceCode.Trim().ToUpperInvariant();

        var service = await _dbContext.Services
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.OrganizationId == organizationId &&
                    item.Code == serviceCode &&
                    !item.IsDeleted,
                cancellationToken);

        if (service is null)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_service_not_found");

            return;
        }

        if (!service.IsActive)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_service_inactive");

            return;
        }

        var serviceAssignedToBrigade = await _dbContext.BrigadeServices
            .AsNoTracking()
            .AnyAsync(
                assignment =>
                    assignment.BrigadeId == visit.BrigadeId &&
                    assignment.ServiceId == service.Id &&
                    assignment.IsAvailable &&
                    !assignment.IsDeleted,
                cancellationToken);

        if (!serviceAssignedToBrigade)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_service_not_available_for_brigade");

            return;
        }

        if (request.ProviderUserId.HasValue)
        {
            var providerExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.ProviderUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!providerExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "service_encounter_provider_user_not_found");

                return;
            }
        }

        var encounterId = syncEvent.EntityId ?? Guid.NewGuid();

        var encounterIdAlreadyExists =
            _dbContext.ServiceEncounters.Local.Any(encounter =>
                encounter.Id == encounterId &&
                encounter.OrganizationId == organizationId &&
                !encounter.IsDeleted) ||
            await _dbContext.ServiceEncounters
                .AsNoTracking()
                .AnyAsync(
                    encounter =>
                        encounter.Id == encounterId &&
                        encounter.OrganizationId == organizationId &&
                        !encounter.IsDeleted,
                    cancellationToken);

        if (encounterIdAlreadyExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_id_already_exists");

            return;
        }

        var encounterFolio = string.IsNullOrWhiteSpace(request.EncounterFolio)
            ? GenerateSyncEncounterFolio(syncEvent)
            : request.EncounterFolio.Trim();

        var normalizedEncounterFolio = encounterFolio.ToUpperInvariant();

        if (acceptedEncounterFoliosInBatch.Contains(normalizedEncounterFolio))
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_folio_duplicate_in_pending_batch");

            return;
        }

        var encounterFolioExists = await _dbContext.ServiceEncounters
            .AsNoTracking()
            .AnyAsync(
                encounter =>
                    encounter.OrganizationId == organizationId &&
                    encounter.EncounterFolio == normalizedEncounterFolio &&
                    !encounter.IsDeleted,
                cancellationToken);

        if (encounterFolioExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_folio_already_exists");

            return;
        }

        var visitServiceKey = $"{request.VisitId:N}:{service.Id:N}";

        if (acceptedEncounterVisitServiceKeysInBatch.Contains(visitServiceKey))
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_duplicate_visit_service_in_pending_batch");

            return;
        }

        var duplicateEncounter = await _dbContext.ServiceEncounters
            .AsNoTracking()
            .AnyAsync(
                encounter =>
                    encounter.VisitId == request.VisitId &&
                    encounter.ServiceId == service.Id &&
                    !encounter.IsDeleted,
                cancellationToken);

        if (duplicateEncounter)
        {
            syncEvent.MarkConflict(
                processedAt,
                "service_encounter_duplicate_visit_service");

            return;
        }

        try
        {
            var encounter = new ServiceEncounter(
                encounterId,
                organizationId,
                normalizedEncounterFolio,
                request.VisitId,
                visit.PatientId,
                visit.BrigadeId,
                service.Id,
                request.ProviderUserId,
                request.StartedAt ?? DateTimeOffset.UtcNow,
                createdOffline: true,
                deviceId: request.DeviceId ?? batch.DeviceId);

            // Pending-batch encounter folio and visit-service keys are reserved only after successful ServiceEncounter construction and reserved atomically.
            var encounterFolioReserved = acceptedEncounterFoliosInBatch.Add(normalizedEncounterFolio);

            if (!encounterFolioReserved)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "service_encounter_folio_duplicate_in_pending_batch");

                return;
            }

            var encounterVisitServiceKeyReserved = acceptedEncounterVisitServiceKeysInBatch.Add(visitServiceKey);

            if (!encounterVisitServiceKeyReserved)
            {
                acceptedEncounterFoliosInBatch.Remove(normalizedEncounterFolio);

                syncEvent.MarkConflict(
                    processedAt,
                    "service_encounter_duplicate_visit_service_in_pending_batch");

                return;
            }

            _dbContext.ServiceEncounters.Add(encounter);

            syncEvent.Accept(
                processedAt,
                encounter.Id);
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

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Vital signs",
                PayloadJsonOptions,
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

    private async Task HandleFormResponseEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<Guid> acceptedFormResponseIdsInBatch,
        ISet<string> acceptedFormResponseEncounterTemplateKeysInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_operation_not_implemented");

            return;
        }

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Form response",
                PayloadJsonOptions,
                out CreateFormResponseRequest? request,
                out var payloadRejectionReason))
        {
            syncEvent.Reject(
                processedAt,
                payloadRejectionReason);

            return;
        }

        if (request.EncounterId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "EncounterId is required for form response sync.");

            return;
        }

        if (request.FormTemplateId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "FormTemplateId is required for form response sync.");

            return;
        }

        if (string.IsNullOrWhiteSpace(request.ResponseJson))
        {
            syncEvent.Reject(
                processedAt,
                "ResponseJson is required for form response sync.");

            return;
        }

        try
        {
            using var responseDocument = JsonDocument.Parse(request.ResponseJson);

            if (responseDocument.RootElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                syncEvent.Reject(
                    processedAt,
                    "Form response JSON root is empty.");

                return;
            }
        }
        catch (JsonException)
        {
            syncEvent.Reject(
                processedAt,
                "Form response JSON is invalid.");

            return;
        }

        ServiceEncounter? trackedEncounter = _dbContext.ServiceEncounters.Local.FirstOrDefault(encounter =>
            encounter.Id == request.EncounterId &&
            encounter.OrganizationId == organizationId &&
            !encounter.IsDeleted);

        var encounter = trackedEncounter ?? await _dbContext.ServiceEncounters
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.EncounterId &&
                    item.OrganizationId == organizationId &&
                    !item.IsDeleted,
                cancellationToken);

        if (encounter is null)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_encounter_not_found");

            return;
        }

        if (encounter.BrigadeId != batch.BrigadeId)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_brigade_mismatch");

            return;
        }

        var submittedAt = request.SubmittedAt ?? DateTimeOffset.UtcNow;

        FormTemplate? trackedTemplate = _dbContext.FormTemplates.Local.FirstOrDefault(template =>
            template.Id == request.FormTemplateId &&
            template.OrganizationId == organizationId &&
            template.ServiceId == encounter.ServiceId &&
            !template.IsDeleted);

        var formTemplate = trackedTemplate ?? await _dbContext.FormTemplates
            .AsNoTracking()
            .SingleOrDefaultAsync(
                template =>
                    template.Id == request.FormTemplateId &&
                    template.OrganizationId == organizationId &&
                    template.ServiceId == encounter.ServiceId &&
                    !template.IsDeleted,
                cancellationToken);

        if (formTemplate is null)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_template_not_found");

            return;
        }

        if (!formTemplate.IsActive)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_template_inactive");

            return;
        }

        if (formTemplate.EffectiveFrom.HasValue &&
            submittedAt < formTemplate.EffectiveFrom.Value)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_template_not_yet_effective");

            return;
        }

        if (formTemplate.EffectiveTo.HasValue &&
            submittedAt > formTemplate.EffectiveTo.Value)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_template_expired");

            return;
        }

        if (request.SubmittedByUserId.HasValue)
        {
            var submittedByUserExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.SubmittedByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!submittedByUserExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "form_response_submitted_by_user_not_found");

                return;
            }
        }

        var formResponseId = syncEvent.EntityId ?? Guid.NewGuid();

        var formResponseIdAlreadyExists =
            acceptedFormResponseIdsInBatch.Contains(formResponseId) ||
            _dbContext.FormResponses.Local.Any(response =>
                response.Id == formResponseId &&
                response.OrganizationId == organizationId &&
                !response.IsDeleted) ||
            await _dbContext.FormResponses
                .AsNoTracking()
                .AnyAsync(
                    response =>
                        response.Id == formResponseId &&
                        response.OrganizationId == organizationId &&
                        !response.IsDeleted,
                    cancellationToken);

        if (formResponseIdAlreadyExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_id_already_exists");

            return;
        }

        var encounterTemplateKey = $"{request.EncounterId:N}:{request.FormTemplateId:N}";

        if (acceptedFormResponseEncounterTemplateKeysInBatch.Contains(encounterTemplateKey))
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_duplicate_encounter_template_in_pending_batch");

            return;
        }

        var duplicateResponseExists = await _dbContext.FormResponses
            .AsNoTracking()
            .AnyAsync(
                response =>
                    response.EncounterId == request.EncounterId &&
                    response.FormTemplateId == request.FormTemplateId &&
                    !response.IsDeleted,
                cancellationToken);

        if (duplicateResponseExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "form_response_duplicate_encounter_template");

            return;
        }

        try
        {
            var formResponse = new FormResponse(
                formResponseId,
                organizationId,
                request.EncounterId,
                request.FormTemplateId,
                request.ResponseJson,
                createdOffline: true,
                deviceId: request.DeviceId ?? batch.DeviceId);

            if (request.SubmittedByUserId.HasValue)
            {
                formResponse.Complete(
                    request.SubmittedByUserId.Value,
                    submittedAt);
            }

            // Pending-batch form response id and encounter-template keys are reserved only after successful FormResponse construction and reserved atomically.
            var formResponseIdReserved = acceptedFormResponseIdsInBatch.Add(formResponseId);

            if (!formResponseIdReserved)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "form_response_duplicate_in_pending_batch");

                return;
            }

            var formResponseEncounterTemplateKeyReserved = acceptedFormResponseEncounterTemplateKeysInBatch.Add(encounterTemplateKey);

            if (!formResponseEncounterTemplateKeyReserved)
            {
                acceptedFormResponseIdsInBatch.Remove(formResponseId);

                syncEvent.MarkConflict(
                    processedAt,
                    "form_response_duplicate_encounter_template_in_pending_batch");

                return;
            }

            _dbContext.FormResponses.Add(formResponse);

            syncEvent.Accept(
                processedAt,
                formResponse.Id);
        }
        catch (DomainException exception)
        {
            syncEvent.Reject(
                processedAt,
                exception.Message);
        }
    }
    private async Task HandleConsentDocumentEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<Guid> acceptedConsentDocumentIdsInBatch,
        ISet<string> acceptedConsentDocumentKeysInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "consent_document_operation_not_implemented");

            return;
        }

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Consent document",
                PayloadJsonOptions,
                out CreateConsentDocumentRequest? request,
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
                "PatientId is required for consent document sync.");

            return;
        }

        if (string.IsNullOrWhiteSpace(request.ConsentType))
        {
            syncEvent.Reject(
                processedAt,
                "ConsentType is required for consent document sync.");

            return;
        }

        if (string.IsNullOrWhiteSpace(request.DocumentVersion))
        {
            syncEvent.Reject(
                processedAt,
                "DocumentVersion is required for consent document sync.");

            return;
        }

        if (string.IsNullOrWhiteSpace(request.SignatureDataUrl))
        {
            syncEvent.Reject(
                processedAt,
                "SignatureDataUrl is required for consent document sync.");

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
                "consent_document_patient_not_found");

            return;
        }

        if (request.VisitId.HasValue)
        {
            var visitExists =
                _dbContext.PatientVisits.Local.Any(visit =>
                    visit.Id == request.VisitId.Value &&
                    visit.OrganizationId == organizationId &&
                    visit.PatientId == request.PatientId &&
                    visit.BrigadeId == batch.BrigadeId &&
                    !visit.IsDeleted) ||
                await _dbContext.PatientVisits
                    .AsNoTracking()
                    .AnyAsync(
                        visit =>
                            visit.Id == request.VisitId.Value &&
                            visit.OrganizationId == organizationId &&
                            visit.PatientId == request.PatientId &&
                            visit.BrigadeId == batch.BrigadeId &&
                            !visit.IsDeleted,
                        cancellationToken);

            if (!visitExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "consent_document_visit_not_found");

                return;
            }
        }

        if (request.SignedByUserId.HasValue)
        {
            var signedByUserExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.SignedByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!signedByUserExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "consent_document_signed_by_user_not_found");

                return;
            }
        }

        var consentDocumentId = syncEvent.EntityId ?? Guid.NewGuid();

        var consentDocumentIdAlreadyExists =
            acceptedConsentDocumentIdsInBatch.Contains(consentDocumentId) ||
            _dbContext.Set<ConsentDocument>().Local.Any(document =>
                document.Id == consentDocumentId &&
                document.OrganizationId == organizationId &&
                !document.IsDeleted) ||
            await _dbContext.Set<ConsentDocument>()
                .AsNoTracking()
                .AnyAsync(
                    document =>
                        document.Id == consentDocumentId &&
                        document.OrganizationId == organizationId &&
                        !document.IsDeleted,
                    cancellationToken);

        if (consentDocumentIdAlreadyExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "consent_document_id_already_exists");

            return;
        }

        var normalizedConsentType = request.ConsentType.Trim().ToUpperInvariant();
        var normalizedDocumentVersion = request.DocumentVersion.Trim();

        var consentDocumentKey =
            $"{request.PatientId:N}:{(request.VisitId.HasValue ? request.VisitId.Value.ToString("N") : "no_visit")}:{normalizedConsentType}:{normalizedDocumentVersion}";

        if (acceptedConsentDocumentKeysInBatch.Contains(consentDocumentKey))
        {
            syncEvent.MarkConflict(
                processedAt,
                "consent_document_duplicate_patient_visit_type_version_in_pending_batch");

            return;
        }

        var duplicateConsentExists =
            _dbContext.Set<ConsentDocument>().Local.Any(document =>
                document.OrganizationId == organizationId &&
                document.PatientId == request.PatientId &&
                document.VisitId == request.VisitId &&
                document.ConsentType == normalizedConsentType &&
                document.DocumentVersion == normalizedDocumentVersion &&
                !document.IsDeleted) ||
            await _dbContext.Set<ConsentDocument>()
                .AsNoTracking()
                .AnyAsync(
                    document =>
                        document.OrganizationId == organizationId &&
                        document.PatientId == request.PatientId &&
                        document.VisitId == request.VisitId &&
                        document.ConsentType == normalizedConsentType &&
                        document.DocumentVersion == normalizedDocumentVersion &&
                        !document.IsDeleted,
                    cancellationToken);

        if (duplicateConsentExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "consent_document_duplicate_patient_visit_type_version");

            return;
        }

        try
        {
            var signedAt = request.SignedAt ?? DateTimeOffset.UtcNow;

            var consentDocument = CreateConsentDocumentForSync(
                consentDocumentId,
                organizationId,
                request,
                normalizedConsentType,
                normalizedDocumentVersion,
                signedAt,
                request.DeviceId ?? batch.DeviceId);

            // Pending-batch consent document id and patient-visit-type-version keys are reserved only after successful ConsentDocument construction and reserved atomically.
            var consentDocumentIdReserved = acceptedConsentDocumentIdsInBatch.Add(consentDocumentId);

            if (!consentDocumentIdReserved)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "consent_document_duplicate_in_pending_batch");

                return;
            }

            var consentDocumentKeyReserved = acceptedConsentDocumentKeysInBatch.Add(consentDocumentKey);

            if (!consentDocumentKeyReserved)
            {
                acceptedConsentDocumentIdsInBatch.Remove(consentDocumentId);

                syncEvent.MarkConflict(
                    processedAt,
                    "consent_document_duplicate_patient_visit_type_version_in_pending_batch");

                return;
            }

            _dbContext.Set<ConsentDocument>().Add(consentDocument);

            syncEvent.Accept(
                processedAt,
                consentDocument.Id);
        }
        catch (Exception exception) when (exception is DomainException or ArgumentException or TargetInvocationException)
        {
            syncEvent.Reject(
                processedAt,
                exception.InnerException?.Message ?? exception.Message);
        }
    }

    private static ConsentDocument CreateConsentDocumentForSync(
        Guid id,
        Guid organizationId,
        CreateConsentDocumentRequest request,
        string normalizedConsentType,
        string normalizedDocumentVersion,
        DateTimeOffset signedAt,
        Guid? deviceId)
    {
        var consentDocument = (ConsentDocument)Activator.CreateInstance(
            typeof(ConsentDocument),
            nonPublic: true)!;

        SetConsentPropertyIfExists(consentDocument, "Id", id);
        SetConsentPropertyIfExists(consentDocument, "OrganizationId", organizationId);
        SetConsentPropertyIfExists(consentDocument, "PatientId", request.PatientId);
        SetConsentPropertyIfExists(consentDocument, "VisitId", request.VisitId);
        SetConsentPropertyIfExists(consentDocument, "ConsentType", normalizedConsentType);
        SetConsentPropertyIfExists(consentDocument, "DocumentVersion", normalizedDocumentVersion);
        SetConsentPropertyIfExists(consentDocument, "DocumentTextSnapshot", request.DocumentTextSnapshot?.Trim());
        SetConsentPropertyIfExists(consentDocument, "SignatureDataUrl", request.SignatureDataUrl?.Trim());
        SetConsentPropertyIfExists(consentDocument, "GuardianFullName", request.GuardianFullName?.Trim());
        SetConsentPropertyIfExists(consentDocument, "GuardianRelationship", request.GuardianRelationship?.Trim());
        SetConsentPropertyIfExists(consentDocument, "SignedByUserId", request.SignedByUserId);
        SetConsentPropertyIfExists(consentDocument, "SignedAt", signedAt);
        SetConsentPropertyIfExists(consentDocument, "CreatedOffline", true);
        SetConsentPropertyIfExists(consentDocument, "DeviceId", deviceId);
        SetConsentPropertyIfExists(consentDocument, "SyncStatus", "Pending");
        SetConsentPropertyIfExists(consentDocument, "CreatedAt", DateTimeOffset.UtcNow);
        SetConsentPropertyIfExists(consentDocument, "IsDeleted", false);

        return consentDocument;
    }

    private static void SetConsentPropertyIfExists(
        object instance,
        string propertyName,
        object? value)
    {
        var property = instance
            .GetType()
            .GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null || !property.CanWrite)
        {
            return;
        }

        property.SetValue(instance, value);
    }
    private async Task HandleMedicalReferralEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<Guid> acceptedMedicalReferralIdsInBatch,
        ISet<string> acceptedMedicalReferralFoliosInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_operation_not_implemented");

            return;
        }

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Medical referral",
                PayloadJsonOptions,
                out CreateMedicalReferralRequest? request,
                out var payloadRejectionReason))
        {
            syncEvent.Reject(
                processedAt,
                payloadRejectionReason);

            return;
        }

        if (request.EncounterId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "EncounterId is required for medical referral sync.");

            return;
        }

        if (string.IsNullOrWhiteSpace(request.ReferralReason))
        {
            syncEvent.Reject(
                processedAt,
                "ReferralReason is required for medical referral sync.");

            return;
        }

        if (request.ProviderSignatureId.HasValue)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_provider_signature_not_supported_until_document_signature_handler");

            return;
        }

        ServiceEncounter? trackedEncounter = _dbContext.ServiceEncounters.Local.FirstOrDefault(encounter =>
            encounter.Id == request.EncounterId &&
            encounter.OrganizationId == organizationId &&
            !encounter.IsDeleted);

        var encounter = trackedEncounter ?? await _dbContext.ServiceEncounters
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.EncounterId &&
                    item.OrganizationId == organizationId &&
                    !item.IsDeleted,
                cancellationToken);

        if (encounter is null)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_encounter_not_found");

            return;
        }

        if (encounter.BrigadeId != batch.BrigadeId)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_brigade_mismatch");

            return;
        }

        var patientExists =
            _dbContext.Patients.Local.Any(patient =>
                patient.Id == encounter.PatientId &&
                patient.OrganizationId == organizationId &&
                !patient.IsDeleted) ||
            await _dbContext.Patients
                .AsNoTracking()
                .AnyAsync(
                    patient =>
                        patient.Id == encounter.PatientId &&
                        patient.OrganizationId == organizationId &&
                        !patient.IsDeleted,
                    cancellationToken);

        if (!patientExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_patient_not_found");

            return;
        }

        if (request.ReferredByUserId.HasValue)
        {
            var referredByUserExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.ReferredByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!referredByUserExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "medical_referral_referred_by_user_not_found");

                return;
            }
        }

        var medicalReferralId = syncEvent.EntityId ?? Guid.NewGuid();

        // Medical referral id duplicate checks include soft-deleted rows because primary key uniqueness is not filtered by IsDeleted.
        var medicalReferralIdAlreadyExists =
            acceptedMedicalReferralIdsInBatch.Contains(medicalReferralId) ||
            _dbContext.Set<MedicalReferral>().Local.Any(referral =>
                referral.Id == medicalReferralId &&
                referral.OrganizationId == organizationId) ||
            await _dbContext.Set<MedicalReferral>()
                .AsNoTracking()
                .AnyAsync(
                    referral =>
                        referral.Id == medicalReferralId &&
                        referral.OrganizationId == organizationId,
                    cancellationToken);

        if (medicalReferralIdAlreadyExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_id_already_exists");

            return;
        }

        var referralFolio = string.IsNullOrWhiteSpace(request.ReferralFolio)
            ? GenerateSyncMedicalReferralFolio(syncEvent)
            : request.ReferralFolio.Trim();

        var normalizedReferralFolio = referralFolio.ToUpperInvariant();

        if (acceptedMedicalReferralFoliosInBatch.Contains(normalizedReferralFolio))
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_folio_duplicate_in_pending_batch");

            return;
        }

        // Medical referral folio duplicate checks include soft-deleted rows because database unique index is not filtered by IsDeleted.
        var referralFolioExists =
            _dbContext.Set<MedicalReferral>().Local.Any(referral =>
                referral.OrganizationId == organizationId &&
                referral.ReferralFolio == normalizedReferralFolio) ||
            await _dbContext.Set<MedicalReferral>()
                .AsNoTracking()
                .AnyAsync(
                    referral =>
                        referral.OrganizationId == organizationId &&
                        referral.ReferralFolio == normalizedReferralFolio,
                    cancellationToken);

        if (referralFolioExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medical_referral_folio_already_exists");

            return;
        }

        try
        {
            var medicalReferral = new MedicalReferral(
                medicalReferralId,
                organizationId,
                request.EncounterId,
                encounter.PatientId,
                normalizedReferralFolio,
                request.ReferralReason,
                request.DestinationInstitution,
                request.Priority,
                request.ReferredByUserId);

            // Pending-batch medical referral id and referral folio are reserved only after successful MedicalReferral construction and reserved atomically.
            var medicalReferralIdReserved = acceptedMedicalReferralIdsInBatch.Add(medicalReferralId);

            if (!medicalReferralIdReserved)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "medical_referral_duplicate_in_pending_batch");

                return;
            }

            var medicalReferralFolioReserved = acceptedMedicalReferralFoliosInBatch.Add(normalizedReferralFolio);

            if (!medicalReferralFolioReserved)
            {
                acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId);

                syncEvent.MarkConflict(
                    processedAt,
                    "medical_referral_folio_duplicate_in_pending_batch");

                return;
            }

            _dbContext.Set<MedicalReferral>().Add(medicalReferral);

            syncEvent.Accept(
                processedAt,
                medicalReferral.Id);
        }
        catch (DomainException exception)
        {
            syncEvent.Reject(
                processedAt,
                exception.Message);
        }
    }
    private async Task HandleMedicationDeliveryEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<Guid> acceptedMedicationDeliveryIdsInBatch,
        CancellationToken cancellationToken)
    {
        if (syncEvent.Operation != SyncOperation.Create)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medication_delivery_operation_not_implemented");

            return;
        }

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Medication delivery",
                PayloadJsonOptions,
                out CreateMedicationDeliveryRequest? request,
                out var payloadRejectionReason))
        {
            syncEvent.Reject(
                processedAt,
                payloadRejectionReason);

            return;
        }

        if (request.EncounterId == Guid.Empty)
        {
            syncEvent.Reject(
                processedAt,
                "EncounterId is required for medication delivery sync.");

            return;
        }

        if (string.IsNullOrWhiteSpace(request.MedicationName))
        {
            syncEvent.Reject(
                processedAt,
                "MedicationName is required for medication delivery sync.");

            return;
        }

        if (request.SignatureId.HasValue)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medication_delivery_signature_not_supported_until_document_signature_handler");

            return;
        }

        ServiceEncounter? trackedEncounter = _dbContext.ServiceEncounters.Local.FirstOrDefault(encounter =>
            encounter.Id == request.EncounterId &&
            encounter.OrganizationId == organizationId &&
            !encounter.IsDeleted);

        var encounter = trackedEncounter ?? await _dbContext.ServiceEncounters
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.EncounterId &&
                    item.OrganizationId == organizationId &&
                    !item.IsDeleted,
                cancellationToken);

        if (encounter is null)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medication_delivery_encounter_not_found");

            return;
        }

        if (encounter.BrigadeId != batch.BrigadeId)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medication_delivery_brigade_mismatch");

            return;
        }

        var patientExists =
            _dbContext.Patients.Local.Any(patient =>
                patient.Id == encounter.PatientId &&
                patient.OrganizationId == organizationId &&
                !patient.IsDeleted) ||
            await _dbContext.Patients
                .AsNoTracking()
                .AnyAsync(
                    patient =>
                        patient.Id == encounter.PatientId &&
                        patient.OrganizationId == organizationId &&
                        !patient.IsDeleted,
                    cancellationToken);

        if (!patientExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medication_delivery_patient_not_found");

            return;
        }

        if (request.MarkAsDelivered && !request.DeliveredByUserId.HasValue)
        {
            syncEvent.Reject(
                processedAt,
                "DeliveredByUserId is required when medication delivery is marked as delivered.");

            return;
        }

        if (request.DeliveredByUserId.HasValue)
        {
            var deliveredByUserExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.DeliveredByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!deliveredByUserExists)
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "medication_delivery_delivered_by_user_not_found");

                return;
            }
        }

        var medicationDeliveryId = syncEvent.EntityId ?? Guid.NewGuid();

        // Medication delivery id duplicate checks include globally duplicated ids because primary key uniqueness is not tenant-scoped.
        var medicationDeliveryIdAlreadyExists =
            acceptedMedicationDeliveryIdsInBatch.Contains(medicationDeliveryId) ||
            _dbContext.Set<MedicationDelivery>().Local.Any(delivery =>
                delivery.Id == medicationDeliveryId) ||
            await _dbContext.Set<MedicationDelivery>()
                .AsNoTracking()
                .AnyAsync(
                    delivery =>
                        delivery.Id == medicationDeliveryId,
                    cancellationToken);

        if (medicationDeliveryIdAlreadyExists)
        {
            syncEvent.MarkConflict(
                processedAt,
                "medication_delivery_id_already_exists");

            return;
        }

        try
        {
            // Non-delivered medication receipt metadata is preserved through constructor fields instead of silently dropped.
            var medicationDelivery = new MedicationDelivery(
                medicationDeliveryId,
                organizationId,
                request.EncounterId,
                encounter.PatientId,
                request.MedicationName,
                request.Presentation,
                request.Quantity,
                request.LotNumber,
                request.ExpirationDate,
                request.Instructions,
                request.MarkAsDelivered ? null : request.DeliveredByUserId,
                request.MarkAsDelivered ? null : request.ReceivedByName,
                signatureId: null);

            if (request.MarkAsDelivered && request.DeliveredByUserId.HasValue)
            {
                medicationDelivery.MarkDelivered(
                    request.DeliveredByUserId.Value,
                    request.ReceivedByName,
                    signatureId: null);
            }

            // Pending-batch medication delivery id is reserved only after successful MedicationDelivery construction and optional delivered transition.
            if (!acceptedMedicationDeliveryIdsInBatch.Add(medicationDeliveryId))
            {
                syncEvent.MarkConflict(
                    processedAt,
                    "medication_delivery_duplicate_in_pending_batch");

                return;
            }

            _dbContext.Set<MedicationDelivery>().Add(medicationDelivery);

            syncEvent.Accept(
                processedAt,
                medicationDelivery.Id);
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

    private static string GenerateSyncMedicalReferralFolio(SyncEvent syncEvent)
    {
        return $"REF-SYNC-{syncEvent.Id:N}"[..41];
    }

    private static string GenerateSyncEncounterFolio(SyncEvent syncEvent)
    {
        return $"ENC-SYNC-{syncEvent.Id:N}"[..41];
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
