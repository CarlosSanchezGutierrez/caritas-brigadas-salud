using Caritas.Brigadas.Application.Services;
using Caritas.Brigadas.Contracts.Services;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Services;

public sealed class ServiceReadRepository : IServiceReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public ServiceReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ServiceSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Services
            .AsNoTracking()
            .Where(service =>
                service.OrganizationId == organizationId &&
                !service.IsDeleted)
            .OrderBy(service => service.Category)
            .ThenBy(service => service.Name)
            .Select(service => new ServiceSummaryDto
            {
                Id = service.Id,
                OrganizationId = service.OrganizationId,
                Code = service.Code,
                Name = service.Name,
                Category = service.Category,
                Description = service.Description,
                RequiresConsent = service.RequiresConsent,
                RequiresClinicalNotes = service.RequiresClinicalNotes,
                RequiresFollowUpOption = service.RequiresFollowUpOption,
                RequiresReferralOption = service.RequiresReferralOption,
                IsSensitive = service.IsSensitive,
                Status = service.Status,
                IsActive = service.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
