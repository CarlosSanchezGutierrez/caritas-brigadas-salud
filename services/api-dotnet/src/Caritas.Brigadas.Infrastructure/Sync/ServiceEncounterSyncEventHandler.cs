using System.Text.Json;
using Caritas.Brigadas.Contracts.ServiceEncounters;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class ServiceEncounterSyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public ServiceEncounterSyncEventHandler(
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
                _payloadJsonOptions,
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

    private static string GenerateSyncEncounterFolio(SyncEvent syncEvent)
    {
        return $"ENC-SYNC-{syncEvent.Id:N}"[..41];
    }
}
