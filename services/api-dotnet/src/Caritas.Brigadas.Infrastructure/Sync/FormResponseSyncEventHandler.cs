using System.Text.Json;
using Caritas.Brigadas.Contracts.FormResponses;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class FormResponseSyncEventHandler
{
    private readonly CaritasDbContext _dbContext;
    private readonly JsonSerializerOptions _payloadJsonOptions;

    public FormResponseSyncEventHandler(
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
                _payloadJsonOptions,
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
}
