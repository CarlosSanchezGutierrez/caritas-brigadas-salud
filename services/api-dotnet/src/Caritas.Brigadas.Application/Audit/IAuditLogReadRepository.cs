using Caritas.Brigadas.Contracts.Audit;

namespace Caritas.Brigadas.Application.Audit;

public interface IAuditLogReadRepository
{
    Task<IReadOnlyCollection<AuditLogSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<AuditLogSummaryDto?> GetByIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default);
}
