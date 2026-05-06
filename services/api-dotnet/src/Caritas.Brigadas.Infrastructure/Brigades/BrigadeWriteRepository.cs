using Caritas.Brigadas.Application.Brigades;
using Caritas.Brigadas.Contracts.Brigades;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Brigades;

public sealed class BrigadeWriteRepository : IBrigadeWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public BrigadeWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BrigadeSummaryDto> CreateAsync(
        Guid organizationId,
        CreateBrigadeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ScheduledDate == default)
        {
            throw new DomainException("Scheduled date is required.");
        }

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

        if (request.CommunityId.HasValue)
        {
            var communityExists = await _dbContext.Communities
                .AsNoTracking()
                .AnyAsync(
                    community =>
                        community.Id == request.CommunityId.Value &&
                        community.OrganizationId == organizationId &&
                        !community.IsDeleted,
                    cancellationToken);

            if (!communityExists)
            {
                throw new KeyNotFoundException("Community was not found in this organization.");
            }
        }

        if (request.MobileUnitId.HasValue)
        {
            var mobileUnitExists = await _dbContext.MobileUnits
                .AsNoTracking()
                .AnyAsync(
                    mobileUnit =>
                        mobileUnit.Id == request.MobileUnitId.Value &&
                        mobileUnit.OrganizationId == organizationId &&
                        !mobileUnit.IsDeleted,
                    cancellationToken);

            if (!mobileUnitExists)
            {
                throw new KeyNotFoundException("Mobile unit was not found in this organization.");
            }
        }

        if (request.CoordinatorUserId.HasValue)
        {
            var coordinatorExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.CoordinatorUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!coordinatorExists)
            {
                throw new KeyNotFoundException("Coordinator user was not found in this organization.");
            }
        }

        var normalizedName = request.Name.Trim();
        var normalizedType = request.BrigadeType.Trim();

        var duplicateExists = await _dbContext.Brigades
            .AsNoTracking()
            .AnyAsync(
                brigade =>
                    brigade.OrganizationId == organizationId &&
                    brigade.Name == normalizedName &&
                    brigade.ScheduledDate == request.ScheduledDate &&
                    !brigade.IsDeleted,
                cancellationToken);

        if (duplicateExists)
        {
            throw new InvalidOperationException("A brigade with the same name and scheduled date already exists.");
        }

        var brigade = new Brigade(
            Guid.NewGuid(),
            organizationId,
            normalizedName,
            normalizedType,
            request.ScheduledDate,
            request.CommunityId,
            request.Municipality,
            request.Colony,
            request.LocationText,
            request.MobileUnitId,
            request.CoordinatorUserId);

        _dbContext.Brigades.Add(brigade);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new BrigadeSummaryDto
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
        };
    }
}
