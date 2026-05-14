using System.Reflection;
using System.Text.Json;
using Caritas.Brigadas.Contracts.ConsentDocuments;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class ConsentDocumentSyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public ConsentDocumentSyncEventHandler(
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
                _payloadJsonOptions,
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
}
