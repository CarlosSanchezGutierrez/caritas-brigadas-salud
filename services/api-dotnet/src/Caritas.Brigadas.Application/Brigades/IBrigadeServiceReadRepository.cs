using Caritas.Brigadas.Contracts.Brigades;

namespace Caritas.Brigadas.Application.Brigades;

public interface IBrigadeServiceReadRepository
{
    Task<IReadOnlyCollection<BrigadeServiceAssignmentDto>> ListByBrigadeAsync(
        Guid brigadeId,
        CancellationToken cancellationToken = default);
}
