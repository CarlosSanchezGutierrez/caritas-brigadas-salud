using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Sync;
using Caritas.Brigadas.Contracts.Api;
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
}
