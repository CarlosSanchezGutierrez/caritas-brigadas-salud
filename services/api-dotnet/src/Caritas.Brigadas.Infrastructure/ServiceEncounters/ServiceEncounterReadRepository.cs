using Caritas.Brigadas.Application.ServiceEncounters;
using Caritas.Brigadas.Contracts.ServiceEncounters;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.ServiceEncounters;

public sealed class ServiceEncounterReadRepository : IServiceEncounterReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public ServiceEncounterReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ServiceEncounterSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ServiceEncounters
            .AsNoTracking()
            .Where(encounter =>
                encounter.OrganizationId == organizationId &&
                !encounter.IsDeleted)
            .Join(
                _dbContext.PatientVisits.AsNoTracking(),
                encounter => encounter.VisitId,
                visit => visit.Id,
                (encounter, visit) => new { encounter, visit })
            .Join(
                _dbContext.Services.AsNoTracking(),
                item => item.encounter.ServiceId,
                service => service.Id,
                (item, service) => new ServiceEncounterSummaryDto
                {
                    Id = item.encounter.Id,
                    OrganizationId = item.encounter.OrganizationId,
                    EncounterFolio = item.encounter.EncounterFolio,
                    VisitId = item.encounter.VisitId,
                    PatientId = item.visit.PatientId,
                    BrigadeId = item.visit.BrigadeId,
                    ServiceId = item.encounter.ServiceId,
                    ServiceCode = service.Code,
                    ServiceName = service.Name,
                    ProviderUserId = item.encounter.ProviderUserId,
                    StartedAt = item.encounter.StartedAt,
                    CompletedAt = null,
                    Status = item.encounter.Status.ToString(),
                    CreatedOffline = item.encounter.CreatedOffline,
                    DeviceId = item.encounter.DeviceId,
                    SyncStatus = item.encounter.SyncStatus.ToString(),
                    IsActive = item.encounter.IsActive,
                    IsCompleted = item.encounter.IsCompleted,
                    NeedsReview = item.encounter.NeedsReview
                })
            .OrderByDescending(encounter => encounter.StartedAt)
            .ThenBy(encounter => encounter.EncounterFolio)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceEncounterSummaryDto?> GetByIdAsync(
        Guid encounterId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ServiceEncounters
            .AsNoTracking()
            .Where(encounter =>
                encounter.Id == encounterId &&
                !encounter.IsDeleted)
            .Join(
                _dbContext.PatientVisits.AsNoTracking(),
                encounter => encounter.VisitId,
                visit => visit.Id,
                (encounter, visit) => new { encounter, visit })
            .Join(
                _dbContext.Services.AsNoTracking(),
                item => item.encounter.ServiceId,
                service => service.Id,
                (item, service) => new ServiceEncounterSummaryDto
                {
                    Id = item.encounter.Id,
                    OrganizationId = item.encounter.OrganizationId,
                    EncounterFolio = item.encounter.EncounterFolio,
                    VisitId = item.encounter.VisitId,
                    PatientId = item.visit.PatientId,
                    BrigadeId = item.visit.BrigadeId,
                    ServiceId = item.encounter.ServiceId,
                    ServiceCode = service.Code,
                    ServiceName = service.Name,
                    ProviderUserId = item.encounter.ProviderUserId,
                    StartedAt = item.encounter.StartedAt,
                    CompletedAt = null,
                    Status = item.encounter.Status.ToString(),
                    CreatedOffline = item.encounter.CreatedOffline,
                    DeviceId = item.encounter.DeviceId,
                    SyncStatus = item.encounter.SyncStatus.ToString(),
                    IsActive = item.encounter.IsActive,
                    IsCompleted = item.encounter.IsCompleted,
                    NeedsReview = item.encounter.NeedsReview
                })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
