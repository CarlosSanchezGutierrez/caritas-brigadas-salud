using System.Text.Json;
using Caritas.Brigadas.Contracts.MedicalReferrals;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class MedicalReferralSyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public MedicalReferralSyncEventHandler(
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
                _payloadJsonOptions,
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

    private static string GenerateSyncMedicalReferralFolio(SyncEvent syncEvent)
    {
        return $"REF-SYNC-{syncEvent.Id:N}"[..41];
    }
}
