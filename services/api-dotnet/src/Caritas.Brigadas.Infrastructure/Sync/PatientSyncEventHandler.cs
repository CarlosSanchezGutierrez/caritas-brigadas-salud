using System.Text.Json;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class PatientSyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public PatientSyncEventHandler(
        CaritasDbContext dbContext,
        JsonSerializerOptions payloadJsonOptions)
    {
        _dbContext = dbContext;
        _payloadJsonOptions = payloadJsonOptions;
    }

    public async Task HandleAsync(
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

        if (!SyncPayloadReader.TryReadObject(
                syncEvent.PayloadJson,
                "Patient",
                _payloadJsonOptions,
                out CreatePatientRequest? request,
                out var payloadRejectionReason))
        {
            syncEvent.Reject(
                processedAt,
                payloadRejectionReason);

            return;
        }

        var patientId = syncEvent.EntityId ?? Guid.NewGuid();

        var patientIdAlreadyExists = await _dbContext.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.Id == patientId &&
                    patient.OrganizationId == organizationId,
                cancellationToken);

        if (patientIdAlreadyExists ||
            _dbContext.Patients.Local.Any(patient => patient.Id == patientId && patient.OrganizationId == organizationId))
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
}
