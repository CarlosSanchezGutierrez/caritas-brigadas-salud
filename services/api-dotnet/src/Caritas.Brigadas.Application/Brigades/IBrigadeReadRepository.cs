using Caritas.Brigadas.Contracts.Brigades;

namespace Caritas.Brigadas.Application.Brigades;

public interface IBrigadeReadRepository
{
    Task<IReadOnlyCollection<BrigadeSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<BrigadeSummaryDto?> GetByIdAsync(
        Guid brigadeId,
        CancellationToken cancellationToken = default);
}
