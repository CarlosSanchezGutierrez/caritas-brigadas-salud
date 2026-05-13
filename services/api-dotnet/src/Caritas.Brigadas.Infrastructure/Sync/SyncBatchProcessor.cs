using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Sync;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

public sealed class SyncBatchProcessor : ISyncBatchProcessor
{
    private const string SkeletonConflictReason = "sync_processor_skeleton_domain_handlers_not_implemented";

    private readonly CaritasDbContext _dbContext;

    public SyncBatchProcessor(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
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

        var processedAt = DateTimeOffset.UtcNow;
        var processedCount = 0;

        foreach (var syncEvent in pendingEvents)
        {
            syncEvent.MarkProcessing();

            if (!TryValidateEvent(syncEvent, out var rejectionReason))
            {
                syncEvent.Reject(
                    processedAt,
                    rejectionReason ?? "Sync event payload is invalid.");

                processedCount++;
                continue;
            }

            syncEvent.MarkConflict(
                processedAt,
                SkeletonConflictReason);

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
                ? "Sync processor skeleton marked events as conflicts because domain handlers are not implemented yet."
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
            Message = "Sync batch processor skeleton completed without applying clinical domain writes."
        };
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