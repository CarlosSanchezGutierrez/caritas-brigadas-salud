using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Contracts.Audit;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Audit;

public sealed class AuditLogWriteRepository : IAuditLogWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public AuditLogWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuditLogSummaryDto> CreateAsync(
        CreateAuditLogCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Action))
        {
            throw new ArgumentException("Audit action is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.EntityName))
        {
            throw new ArgumentException("Audit entity name is required.", nameof(command));
        }

        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Id == command.OrganizationId &&
                    !organization.IsDeleted,
                cancellationToken);

        if (!organizationExists)
        {
            throw new KeyNotFoundException("Organization was not found.");
        }

        if (command.UserId.HasValue)
        {
            var userExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == command.UserId.Value &&
                        user.OrganizationId == command.OrganizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException("Audit user was not found in this organization.");
            }
        }

        var auditLog = new AuditLog(
            id: Guid.NewGuid(),
            organizationId: command.OrganizationId,
            userId: command.UserId,
            action: command.Action,
            entityName: command.EntityName,
            entityId: command.EntityId,
            detailsJson: command.DetailsJson,
            correlationId: command.CorrelationId,
            ipAddress: command.IpAddress,
            userAgent: command.UserAgent,
            occurredAtUtc: command.OccurredAtUtc ?? DateTimeOffset.UtcNow);

        _dbContext.AuditLogs.Add(auditLog);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(auditLog);
    }

    private static AuditLogSummaryDto MapToDto(AuditLog auditLog)
    {
        return new AuditLogSummaryDto
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
        };
    }
}
