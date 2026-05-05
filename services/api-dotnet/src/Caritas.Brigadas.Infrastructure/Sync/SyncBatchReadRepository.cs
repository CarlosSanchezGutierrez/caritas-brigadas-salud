using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Sync;
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

    public async Task<IReadOnlyCollection<SyncBatchSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SyncBatches
            .AsNoTracking()
            .Where(batch => batch.OrganizationId == organizationId)
            .OrderByDescending(batch => batch.StartedAt)
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
            .ToListAsync(cancellationToken);
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
