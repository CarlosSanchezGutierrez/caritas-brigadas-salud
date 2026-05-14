using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Contracts.ServiceEncounters;
using Caritas.Brigadas.Contracts.FormResponses;
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
    private readonly PatientVisitSyncEventHandler _patientVisitSyncEventHandler;
    private readonly ServiceEncounterSyncEventHandler _serviceEncounterSyncEventHandler;
    private readonly VitalSignsSyncEventHandler _vitalSignsSyncEventHandler;
    private readonly FormResponseSyncEventHandler _formResponseSyncEventHandler;
    private readonly ConsentDocumentSyncEventHandler _consentDocumentSyncEventHandler;
    private readonly MedicalReferralSyncEventHandler _medicalReferralSyncEventHandler;

    public SyncBatchProcessor(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
        _patientSyncEventHandler = new PatientSyncEventHandler(dbContext, PayloadJsonOptions);
        _patientVisitSyncEventHandler = new PatientVisitSyncEventHandler(dbContext, PayloadJsonOptions);
        _serviceEncounterSyncEventHandler = new ServiceEncounterSyncEventHandler(dbContext, PayloadJsonOptions);
        _vitalSignsSyncEventHandler = new VitalSignsSyncEventHandler(dbContext, PayloadJsonOptions);
        _formResponseSyncEventHandler = new FormResponseSyncEventHandler(dbContext, PayloadJsonOptions);
        _consentDocumentSyncEventHandler = new ConsentDocumentSyncEventHandler(dbContext, PayloadJsonOptions);
        _medicalReferralSyncEventHandler = new MedicalReferralSyncEventHandler(dbContext, PayloadJsonOptions);
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
        await _patientVisitSyncEventHandler.HandleAsync(
            organizationId,
            batch,
            syncEvent,
            processedAt,
            acceptedVisitFoliosInBatch,
            cancellationToken);
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
        await _serviceEncounterSyncEventHandler.HandleAsync(
            organizationId,
            batch,
            syncEvent,
            processedAt,
            acceptedEncounterFoliosInBatch,
            acceptedEncounterVisitServiceKeysInBatch,
            cancellationToken);
    }

    private async Task HandleVitalSignsEventAsync(
        Guid organizationId,
        SyncBatch batch,
        SyncEvent syncEvent,
        DateTimeOffset processedAt,
        ISet<Guid> acceptedVitalSignsIdsInBatch,
        CancellationToken cancellationToken)
    {
        await _vitalSignsSyncEventHandler.HandleAsync(
            organizationId,
            batch,
            syncEvent,
            processedAt,
            acceptedVitalSignsIdsInBatch,
            cancellationToken);
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
        await _formResponseSyncEventHandler.HandleAsync(
            organizationId,
            batch,
            syncEvent,
            processedAt,
            acceptedFormResponseIdsInBatch,
            acceptedFormResponseEncounterTemplateKeysInBatch,
            cancellationToken);
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
        await _consentDocumentSyncEventHandler.HandleAsync(
            organizationId,
            batch,
            syncEvent,
            processedAt,
            acceptedConsentDocumentIdsInBatch,
            acceptedConsentDocumentKeysInBatch,
            cancellationToken);
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
        await _medicalReferralSyncEventHandler.HandleAsync(
            organizationId,
            batch,
            syncEvent,
            processedAt,
            acceptedMedicalReferralIdsInBatch,
            acceptedMedicalReferralFoliosInBatch,
            cancellationToken);
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
