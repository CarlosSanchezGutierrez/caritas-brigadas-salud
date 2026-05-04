using Caritas.Brigadas.Application.Communities;
using Caritas.Brigadas.Contracts.Communities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Communities;

public sealed class CommunityReadRepository : ICommunityReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public CommunityReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CommunitySummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities
            .AsNoTracking()
            .Where(community =>
                community.OrganizationId == organizationId &&
                !community.IsDeleted)
            .OrderBy(community => community.Municipality)
            .ThenBy(community => community.Colony)
            .ThenBy(community => community.CommunityName)
            .Select(community => new CommunitySummaryDto
            {
                Id = community.Id,
                OrganizationId = community.OrganizationId,
                State = community.State,
                Municipality = community.Municipality,
                Colony = community.Colony,
                CommunityName = community.CommunityName,
                AddressReference = community.AddressReference,
                RiskLevel = community.RiskLevel,
                Status = community.Status,
                IsActive = community.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CommunitySummaryDto?> GetByIdAsync(
        Guid communityId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities
            .AsNoTracking()
            .Where(community =>
                community.Id == communityId &&
                !community.IsDeleted)
            .Select(community => new CommunitySummaryDto
            {
                Id = community.Id,
                OrganizationId = community.OrganizationId,
                State = community.State,
                Municipality = community.Municipality,
                Colony = community.Colony,
                CommunityName = community.CommunityName,
                AddressReference = community.AddressReference,
                RiskLevel = community.RiskLevel,
                Status = community.Status,
                IsActive = community.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
