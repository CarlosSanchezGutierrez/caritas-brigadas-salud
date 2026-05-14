using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Sync;
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
    private readonly MedicationDeliverySyncEventHandler _medicationDeliverySyncEventHandler;

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
        _medicationDeliverySyncEventHandler = new MedicationDeliverySyncEventHandler(dbContext, PayloadJsonOptions);
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
        await _medicationDeliverySyncEventHandler.HandleAsync(
            organizationId,
            batch,
            syncEvent,
            processedAt,
            acceptedMedicationDeliveryIdsInBatch,
            cancellationToken);
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
