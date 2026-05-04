using Caritas.Brigadas.Application.Brigades;
using Caritas.Brigadas.Contracts.Brigades;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Brigades;

public sealed class BrigadeServiceReadRepository : IBrigadeServiceReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public BrigadeServiceReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<BrigadeServiceAssignmentDto>> ListByBrigadeAsync(
        Guid brigadeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.BrigadeServices
            .AsNoTracking()
            .Where(assignment =>
                assignment.BrigadeId == brigadeId &&
                !assignment.IsDeleted)
            .Join(
                _dbContext.Services.AsNoTracking().Where(service => !service.IsDeleted),
                assignment => assignment.ServiceId,
                service => service.Id,
                (assignment, service) => new BrigadeServiceAssignmentDto
                {
                    Id = assignment.Id,
                    BrigadeId = assignment.BrigadeId,
                    ServiceId = assignment.ServiceId,
                    ServiceCode = service.Code,
                    ServiceName = service.Name,
                    ServiceCategory = service.Category,
                    IsAvailable = assignment.IsAvailable,
                    CapacityEstimate = assignment.CapacityEstimate,
                    AssignedLeadUserId = assignment.AssignedLeadUserId
                })
            .OrderBy(assignment => assignment.ServiceCategory)
            .ThenBy(assignment => assignment.ServiceName)
            .ToListAsync(cancellationToken);
    }
}
