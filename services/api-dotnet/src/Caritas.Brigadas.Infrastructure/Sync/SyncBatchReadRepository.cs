using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Sync;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

public sealed class SyncBatchReadRepository : ISyncBatchReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public SyncBatchReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<SyncBatchSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.NormalizedPageNumber;
        var pageSize = pagination.NormalizedPageSize;

        var query = _dbContext.SyncBatches
            .AsNoTracking()
            .Where(batch => batch.OrganizationId == organizationId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(batch => batch.Id)
            .Skip(pagination.Skip)
            .Take(pageSize)
            .Select(batch => new SyncBatchSummaryDto
            {
                Id = batch.Id,
                OrganizationId = batch.OrganizationId,
                UserId = batch.UserId,
                BrigadeId = batch.BrigadeId,
                DeviceId = batch.DeviceId,
                EventsCount = batch.EventsCount,
                Status = batch.Status.ToString(),
                StartedAt = batch.StartedAt,
                CompletedAt = batch.CompletedAt,
                ErrorSummary = batch.ErrorSummary,
                IsCompleted = batch.IsCompleted
            })
            .ToArrayAsync(cancellationToken);

        return new PaginatedResponse<SyncBatchSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<SyncBatchSummaryDto?> GetByIdAsync(
        Guid syncBatchId,
        CancellationToken cancellationToken = default)
    {
        if (syncBatchId == Guid.Empty)
        {
            throw new ArgumentException("Sync batch id is required.", nameof(syncBatchId));
        }

        return await _dbContext.SyncBatches
            .AsNoTracking()
            .Where(batch => batch.Id == syncBatchId)
            .Select(batch => new SyncBatchSummaryDto
            {
                Id = batch.Id,
                OrganizationId = batch.OrganizationId,
                UserId = batch.UserId,
                BrigadeId = batch.BrigadeId,
                DeviceId = batch.DeviceId,
                EventsCount = batch.EventsCount,
                Status = batch.Status.ToString(),
                StartedAt = batch.StartedAt,
                CompletedAt = batch.CompletedAt,
                ErrorSummary = batch.ErrorSummary,
                IsCompleted = batch.IsCompleted
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<SyncEventSummaryDto>> ListEventsByBatchAsync(
        Guid organizationId,
        Guid syncBatchId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        if (syncBatchId == Guid.Empty)
        {
            throw new ArgumentException("Sync batch id is required.", nameof(syncBatchId));
        }

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.NormalizedPageNumber;
        var pageSize = pagination.NormalizedPageSize;

        var query = _dbContext.SyncEvents
            .AsNoTracking()
            .Where(syncEvent =>
                syncEvent.OrganizationId == organizationId &&
                syncEvent.SyncBatchId == syncBatchId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(syncEvent => syncEvent.ReceivedAtServer)
            .ThenBy(syncEvent => syncEvent.Id)
            .Skip(pagination.Skip)
            .Take(pageSize)
            .Select(syncEvent => new SyncEventSummaryDto
            {
                Id = syncEvent.Id,
                SyncBatchId = syncEvent.SyncBatchId,
                OrganizationId = syncEvent.OrganizationId,
                LocalEventId = syncEvent.LocalEventId,
                IdempotencyKey = syncEvent.IdempotencyKey,
                EntityType = syncEvent.EntityType,
                EntityId = syncEvent.EntityId,
                Operation = syncEvent.Operation,
                Status = syncEvent.Status,
                ErrorMessage = syncEvent.ErrorMessage,
                ConflictReason = syncEvent.ConflictReason,
                CreatedAtDevice = syncEvent.CreatedAtDevice,
                ReceivedAtServer = syncEvent.ReceivedAtServer,
                ProcessedAt = syncEvent.ProcessedAt,
                IsPending = syncEvent.Status == SyncEventStatus.Pending,
                IsAccepted = syncEvent.Status == SyncEventStatus.Accepted,
                IsRejected = syncEvent.Status == SyncEventStatus.Rejected,
                IsConflict = syncEvent.Status == SyncEventStatus.Conflict})
            .ToArrayAsync(cancellationToken);

        return new PaginatedResponse<SyncEventSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}