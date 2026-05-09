using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Audit;

namespace Caritas.Brigadas.Application.Audit;

public interface IAuditLogReadRepository
{
    Task<PaginatedResponse<AuditLogSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<AuditLogSummaryDto?> GetByIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default);
}