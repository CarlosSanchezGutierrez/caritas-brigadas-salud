using Caritas.Brigadas.Application.MobileUnits;
using Caritas.Brigadas.Contracts.MobileUnits;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.MobileUnits;

public sealed class MobileUnitWriteRepository : IMobileUnitWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public MobileUnitWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MobileUnitSummaryDto> CreateAsync(
        Guid organizationId,
        CreateMobileUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Id == organizationId &&
                    !organization.IsDeleted,
                cancellationToken);

        if (!organizationExists)
        {
            throw new KeyNotFoundException("Organization was not found.");
        }

        var normalizedName = request.Name.Trim();

        var duplicateExists = await _dbContext.MobileUnits
            .AsNoTracking()
            .AnyAsync(
                unit =>
                    unit.OrganizationId == organizationId &&
                    unit.Name == normalizedName &&
                    !unit.IsDeleted,
                cancellationToken);

        if (duplicateExists)
        {
            throw new InvalidOperationException("A mobile unit with the same name already exists.");
        }

        var unit = new MobileUnit(
            Guid.NewGuid(),
            organizationId,
            normalizedName,
            request.UnitType,
            request.PlateNumber,
            request.Description);

        _dbContext.MobileUnits.Add(unit);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MobileUnitSummaryDto
        {
            Id = unit.Id,
            OrganizationId = unit.OrganizationId,
            Name = unit.Name,
            UnitType = unit.UnitType,
            PlateNumber = unit.PlateNumber,
            Description = unit.Description,
            Status = unit.Status,
            IsActive = unit.IsActive
        };
    }
}
