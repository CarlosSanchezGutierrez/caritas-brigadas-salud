using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Contracts.Audit;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Audit;

public sealed class AuditLogReadRepository : IAuditLogReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public AuditLogReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AuditLogSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        return await _dbContext.AuditLogs
            .AsNoTracking()
            .Where(auditLog => auditLog.OrganizationId == organizationId)
            .OrderByDescending(auditLog => auditLog.OccurredAtUtc)
            .Take(250)
            .Select(auditLog => new AuditLogSummaryDto
            {
                Id = auditLog.Id,
                OrganizationId = auditLog.OrganizationId,
                EntityName = auditLog.EntityName,
                EntityId = auditLog.EntityId,
                Action = auditLog.Action,
                UserId = auditLog.UserId,
                OccurredAtUtc = auditLog.OccurredAtUtc,
                CorrelationId = auditLog.CorrelationId,
                IpAddress = auditLog.IpAddress,
                DetailsJson = auditLog.DetailsJson
            })
            .ToArrayAsync(cancellationToken);
    }

    public async Task<AuditLogSummaryDto?> GetByIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default)
    {
        if (auditLogId == Guid.Empty)
        {
            throw new ArgumentException("Audit log id is required.", nameof(auditLogId));
        }

        var auditLog = await _dbContext.AuditLogs
            .AsNoTracking()
            .Where(item => item.Id == auditLogId)
            .Select(item => new AuditLogSummaryDto
            {
                Id = item.Id,
                OrganizationId = item.OrganizationId,
                EntityName = item.EntityName,
                EntityId = item.EntityId,
                Action = item.Action,
                UserId = item.UserId,
                OccurredAtUtc = item.OccurredAtUtc,
                CorrelationId = item.CorrelationId,
                IpAddress = item.IpAddress,
                DetailsJson = item.DetailsJson
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (auditLog is null)
        {
            throw new KeyNotFoundException("Audit log was not found.");
        }

        return auditLog;
    }
}
