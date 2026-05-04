using Caritas.Brigadas.Contracts.ServiceEncounters;

namespace Caritas.Brigadas.Application.ServiceEncounters;

public interface IServiceEncounterReadRepository
{
    Task<IReadOnlyCollection<ServiceEncounterSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<ServiceEncounterSummaryDto?> GetByIdAsync(
        Guid encounterId,
        CancellationToken cancellationToken = default);
}
