using Caritas.Brigadas.Contracts.Sync;

namespace Caritas.Brigadas.Application.Sync;

public interface ISyncBatchProcessor
{
    Task<ProcessSyncBatchResultDto> ProcessAsync(
        Guid organizationId,
        Guid syncBatchId,
        CancellationToken cancellationToken = default);
}