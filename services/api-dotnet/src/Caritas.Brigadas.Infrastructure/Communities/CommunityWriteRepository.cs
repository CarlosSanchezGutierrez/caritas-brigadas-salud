using Caritas.Brigadas.Application.Communities;
using Caritas.Brigadas.Contracts.Communities;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Communities;

public sealed class CommunityWriteRepository : ICommunityWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public CommunityWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommunitySummaryDto> CreateAsync(
        Guid organizationId,
        CreateCommunityRequest request,
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

        var state = string.IsNullOrWhiteSpace(request.State)
            ? "Nuevo León"
            : request.State.Trim();

        var municipality = request.Municipality.Trim();
        var colony = NormalizeOptional(request.Colony);
        var communityName = NormalizeOptional(request.CommunityName);

        var duplicateExists = await _dbContext.Communities
            .AsNoTracking()
            .AnyAsync(
                community =>
                    community.OrganizationId == organizationId &&
                    community.State == state &&
                    community.Municipality == municipality &&
                    community.Colony == colony &&
                    community.CommunityName == communityName &&
                    !community.IsDeleted,
                cancellationToken);

        if (duplicateExists)
        {
            throw new InvalidOperationException("A community with the same location already exists.");
        }

        var community = new Community(
            Guid.NewGuid(),
            organizationId,
            municipality,
            colony,
            communityName,
            state);

        community.UpdateLocation(
            state,
            municipality,
            colony,
            communityName,
            request.AddressReference);

        community.UpdateRiskLevel(request.RiskLevel);

        _dbContext.Communities.Add(community);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommunitySummaryDto
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
        };
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
