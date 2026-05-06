using Caritas.Brigadas.Contracts.Sync;

namespace Caritas.Brigadas.Application.Sync;

public interface ISyncBatchWriteRepository
{
    Task<SyncBatchSummaryDto> CreateAsync(
        Guid organizationId,
        CreateSyncBatchRequest request,
        CancellationToken cancellationToken = default);
}
