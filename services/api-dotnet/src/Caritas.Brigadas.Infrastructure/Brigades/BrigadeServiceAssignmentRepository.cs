using Caritas.Brigadas.Application.Brigades;
using Caritas.Brigadas.Contracts.Brigades;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Brigades;

public sealed class BrigadeServiceAssignmentRepository : IBrigadeServiceAssignmentRepository
{
    private readonly CaritasDbContext _dbContext;

    public BrigadeServiceAssignmentRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BrigadeServiceAssignmentDto> AssignAsync(
        Guid brigadeId,
        AssignBrigadeServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var brigade = await _dbContext.Brigades
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.Id == brigadeId &&
                    !item.IsDeleted,
                cancellationToken);

        if (brigade is null)
        {
            throw new KeyNotFoundException("Brigade was not found.");
        }

        if (brigade.IsClosed)
        {
            throw new InvalidOperationException("Closed brigades cannot receive new service assignments.");
        }

        var serviceCode = request.ServiceCode.Trim().ToUpperInvariant();

        var service = await _dbContext.Services
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.OrganizationId == brigade.OrganizationId &&
                    item.Code == serviceCode &&
                    !item.IsDeleted,
                cancellationToken);

        if (service is null)
        {
            throw new KeyNotFoundException("Service was not found in this organization.");
        }

        if (!service.IsActive)
        {
            throw new InvalidOperationException("Inactive services cannot be assigned to a brigade.");
        }

        if (request.AssignedLeadUserId.HasValue)
        {
            var assignedLeadExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.AssignedLeadUserId.Value &&
                        user.OrganizationId == brigade.OrganizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!assignedLeadExists)
            {
                throw new KeyNotFoundException("Assigned lead user was not found in this organization.");
            }
        }

        var alreadyAssigned = await _dbContext.BrigadeServices
            .AsNoTracking()
            .AnyAsync(
                assignment =>
                    assignment.BrigadeId == brigadeId &&
                    assignment.ServiceId == service.Id &&
                    !assignment.IsDeleted,
                cancellationToken);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException("Service is already assigned to this brigade.");
        }

        var brigadeService = new BrigadeService(
            Guid.NewGuid(),
            brigadeId,
            service.Id,
            request.CapacityEstimate,
            request.AssignedLeadUserId);

        _dbContext.BrigadeServices.Add(brigadeService);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new BrigadeServiceAssignmentDto
        {
            Id = brigadeService.Id,
            BrigadeId = brigadeService.BrigadeId,
            ServiceId = brigadeService.ServiceId,
            ServiceCode = service.Code,
            ServiceName = service.Name,
            ServiceCategory = service.Category,
            IsAvailable = brigadeService.IsAvailable,
            CapacityEstimate = brigadeService.CapacityEstimate,
            AssignedLeadUserId = brigadeService.AssignedLeadUserId
        };
    }
}
