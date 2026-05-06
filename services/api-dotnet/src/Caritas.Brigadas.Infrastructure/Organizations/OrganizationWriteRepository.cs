using Caritas.Brigadas.Application.Organizations;
using Caritas.Brigadas.Contracts.Organizations;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Organizations;

public sealed class OrganizationWriteRepository : IOrganizationWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public OrganizationWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrganizationSummaryDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();

        var exists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Name == normalizedName &&
                    !organization.IsDeleted,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("An organization with the same name already exists.");
        }

        var organization = new Organization(
            Guid.NewGuid(),
            normalizedName,
            request.LegalName,
            request.Rfc);

        organization.UpdateContact(
            request.Address,
            request.Phone,
            request.Email,
            request.Website);

        _dbContext.Organizations.Add(organization);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new OrganizationSummaryDto
        {
            Id = organization.Id,
            Name = organization.Name,
            LegalName = organization.LegalName,
            Rfc = organization.Rfc,
            Email = organization.Email,
            Website = organization.Website,
            Status = organization.Status,
            IsActive = organization.IsActive
        };
    }
}
