using Caritas.Brigadas.Application.PatientVisits;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.PatientVisits;

public sealed class PatientVisitReadRepository : IPatientVisitReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public PatientVisitReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<PatientVisitSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientVisits
            .AsNoTracking()
            .Where(visit =>
                visit.OrganizationId == organizationId &&
                !visit.IsDeleted)
            .OrderByDescending(visit => visit.ArrivalTime)
            .ThenBy(visit => visit.VisitFolio)
            .Select(visit => new PatientVisitSummaryDto
            {
                Id = visit.Id,
                OrganizationId = visit.OrganizationId,
                VisitFolio = visit.VisitFolio,
                PatientId = visit.PatientId,
                BrigadeId = visit.BrigadeId,
                ArrivalTime = visit.ArrivalTime,
                RegisteredByUserId = visit.RegisteredByUserId,
                VisitStatus = visit.VisitStatus.ToString(),
                CreatedOffline = visit.CreatedOffline,
                DeviceId = visit.DeviceId,
                SyncStatus = visit.SyncStatus.ToString(),
                ClosedAt = visit.ClosedAt,
                ClosedByUserId = visit.ClosedByUserId,
                IsActive = visit.IsActive,
                IsClosed = visit.IsClosed,
                NeedsReview = visit.NeedsReview
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PatientVisitSummaryDto?> GetByIdAsync(
        Guid visitId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientVisits
            .AsNoTracking()
            .Where(visit =>
                visit.Id == visitId &&
                !visit.IsDeleted)
            .Select(visit => new PatientVisitSummaryDto
            {
                Id = visit.Id,
                OrganizationId = visit.OrganizationId,
                VisitFolio = visit.VisitFolio,
                PatientId = visit.PatientId,
                BrigadeId = visit.BrigadeId,
                ArrivalTime = visit.ArrivalTime,
                RegisteredByUserId = visit.RegisteredByUserId,
                VisitStatus = visit.VisitStatus.ToString(),
                CreatedOffline = visit.CreatedOffline,
                DeviceId = visit.DeviceId,
                SyncStatus = visit.SyncStatus.ToString(),
                ClosedAt = visit.ClosedAt,
                ClosedByUserId = visit.ClosedByUserId,
                IsActive = visit.IsActive,
                IsClosed = visit.IsClosed,
                NeedsReview = visit.NeedsReview
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
