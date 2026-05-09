using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Sync;

namespace Caritas.Brigadas.Application.Sync;

public interface ISyncBatchReadRepository
{
    Task<PaginatedResponse<SyncBatchSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<SyncBatchSummaryDto?> GetByIdAsync(
        Guid syncBatchId,
        CancellationToken cancellationToken = default);
}