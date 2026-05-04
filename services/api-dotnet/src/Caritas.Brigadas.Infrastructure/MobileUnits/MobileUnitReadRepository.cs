using Caritas.Brigadas.Application.MobileUnits;
using Caritas.Brigadas.Contracts.MobileUnits;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.MobileUnits;

public sealed class MobileUnitReadRepository : IMobileUnitReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public MobileUnitReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<MobileUnitSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.MobileUnits
            .AsNoTracking()
            .Where(unit =>
                unit.OrganizationId == organizationId &&
                !unit.IsDeleted)
            .OrderBy(unit => unit.Name)
            .Select(unit => new MobileUnitSummaryDto
            {
                Id = unit.Id,
                OrganizationId = unit.OrganizationId,
                Name = unit.Name,
                UnitType = unit.UnitType,
                PlateNumber = unit.PlateNumber,
                Description = unit.Description,
                Status = unit.Status,
                IsActive = unit.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<MobileUnitSummaryDto?> GetByIdAsync(
        Guid mobileUnitId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.MobileUnits
            .AsNoTracking()
            .Where(unit =>
                unit.Id == mobileUnitId &&
                !unit.IsDeleted)
            .Select(unit => new MobileUnitSummaryDto
            {
                Id = unit.Id,
                OrganizationId = unit.OrganizationId,
                Name = unit.Name,
                UnitType = unit.UnitType,
                PlateNumber = unit.PlateNumber,
                Description = unit.Description,
                Status = unit.Status,
                IsActive = unit.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
