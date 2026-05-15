using System.Text.Json;
using Caritas.Brigadas.Contracts.MedicationDeliveries;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class MedicationDeliverySyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public MedicationDeliverySyncEventHandler(
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
                _payloadJsonOptions,
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
}
