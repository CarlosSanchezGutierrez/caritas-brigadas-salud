using System.Text.Json;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class PatientVisitSyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public PatientVisitSyncEventHandler(
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
                _payloadJsonOptions,
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

    private static string GenerateSyncVisitFolio(SyncEvent syncEvent)
    {
        return $"VIS-SYNC-{syncEvent.Id:N}"[..41];
    }
}
