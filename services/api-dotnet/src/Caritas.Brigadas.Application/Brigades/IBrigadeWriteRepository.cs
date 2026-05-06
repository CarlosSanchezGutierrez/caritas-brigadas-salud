using Caritas.Brigadas.Contracts.Brigades;

namespace Caritas.Brigadas.Application.Brigades;

public interface IBrigadeWriteRepository
{
    Task<BrigadeSummaryDto> CreateAsync(
        Guid organizationId,
        CreateBrigadeRequest request,
        CancellationToken cancellationToken = default);
}
