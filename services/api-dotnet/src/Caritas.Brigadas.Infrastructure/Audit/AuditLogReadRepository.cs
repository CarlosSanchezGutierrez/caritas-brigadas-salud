using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Contracts.Api;
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

    public async Task<PaginatedResponse<AuditLogSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.NormalizedPageNumber;
        var pageSize = pagination.NormalizedPageSize;

        var query = _dbContext.AuditLogs
            .AsNoTracking()
            .Where(auditLog => auditLog.OrganizationId == organizationId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(auditLog => auditLog.OccurredAtUtc)
            .ThenByDescending(auditLog => auditLog.Id)
            .Skip(pagination.Skip)
            .Take(pageSize)
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

        return new PaginatedResponse<AuditLogSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AuditLogSummaryDto?> GetByIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default)
    {
        if (auditLogId == Guid.Empty)
        {
            throw new ArgumentException("Audit log id is required.", nameof(auditLogId));
        }

        return await _dbContext.AuditLogs
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
            .SingleOrDefaultAsync(cancellationToken);
    }
}