using Caritas.Brigadas.Application.Brigades;
using Caritas.Brigadas.Contracts.Brigades;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Brigades;

public sealed class BrigadeReadRepository : IBrigadeReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public BrigadeReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<BrigadeSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brigades
            .AsNoTracking()
            .Where(brigade =>
                brigade.OrganizationId == organizationId &&
                !brigade.IsDeleted)
            .OrderByDescending(brigade => brigade.ScheduledDate)
            .ThenBy(brigade => brigade.Name)
            .Select(brigade => new BrigadeSummaryDto
            {
                Id = brigade.Id,
                OrganizationId = brigade.OrganizationId,
                Name = brigade.Name,
                BrigadeType = brigade.BrigadeType,
                ScheduledDate = brigade.ScheduledDate,
                StartTime = brigade.StartTime,
                EndTime = brigade.EndTime,
                CommunityId = brigade.CommunityId,
                Municipality = brigade.Municipality,
                Colony = brigade.Colony,
                LocationText = brigade.LocationText,
                MobileUnitId = brigade.MobileUnitId,
                CoordinatorUserId = brigade.CoordinatorUserId,
                Status = brigade.Status,
                IsPlanned = brigade.IsPlanned,
                IsActive = brigade.IsActive,
                IsClosed = brigade.IsClosed
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<BrigadeSummaryDto?> GetByIdAsync(
        Guid brigadeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brigades
            .AsNoTracking()
            .Where(brigade =>
                brigade.Id == brigadeId &&
                !brigade.IsDeleted)
            .Select(brigade => new BrigadeSummaryDto
            {
                Id = brigade.Id,
                OrganizationId = brigade.OrganizationId,
                Name = brigade.Name,
                BrigadeType = brigade.BrigadeType,
                ScheduledDate = brigade.ScheduledDate,
                StartTime = brigade.StartTime,
                EndTime = brigade.EndTime,
                CommunityId = brigade.CommunityId,
                Municipality = brigade.Municipality,
                Colony = brigade.Colony,
                LocationText = brigade.LocationText,
                MobileUnitId = brigade.MobileUnitId,
                CoordinatorUserId = brigade.CoordinatorUserId,
                Status = brigade.Status,
                IsPlanned = brigade.IsPlanned,
                IsActive = brigade.IsActive,
                IsClosed = brigade.IsClosed
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
