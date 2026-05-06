using Caritas.Brigadas.Application.Organizations;
using Caritas.Brigadas.Contracts.Organizations;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Organizations;

public sealed class OrganizationReadRepository : IOrganizationReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public OrganizationReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<OrganizationSummaryDto>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .Where(organization => !organization.IsDeleted)
            .OrderBy(organization => organization.Name)
            .Select(organization => new OrganizationSummaryDto
            {
                Id = organization.Id,
                Name = organization.Name,
                LegalName = organization.LegalName,
                Rfc = organization.Rfc,
                Email = organization.Email,
                Website = organization.Website,
                Status = organization.Status,
                IsActive = organization.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<OrganizationSummaryDto?> GetByIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .Where(organization =>
                organization.Id == organizationId &&
                !organization.IsDeleted)
            .Select(organization => new OrganizationSummaryDto
            {
                Id = organization.Id,
                Name = organization.Name,
                LegalName = organization.LegalName,
                Rfc = organization.Rfc,
                Email = organization.Email,
                Website = organization.Website,
                Status = organization.Status,
                IsActive = organization.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
