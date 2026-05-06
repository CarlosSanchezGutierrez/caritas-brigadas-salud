using Caritas.Brigadas.Contracts.Sync;

namespace Caritas.Brigadas.Application.Sync;

public interface ISyncBatchReadRepository
{
    Task<IReadOnlyCollection<SyncBatchSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<SyncBatchSummaryDto?> GetByIdAsync(
        Guid syncBatchId,
        CancellationToken cancellationToken = default);
}
