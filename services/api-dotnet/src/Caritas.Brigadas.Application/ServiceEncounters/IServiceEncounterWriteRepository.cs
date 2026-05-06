using Caritas.Brigadas.Contracts.ServiceEncounters;

namespace Caritas.Brigadas.Application.ServiceEncounters;

public interface IServiceEncounterWriteRepository
{
    Task<ServiceEncounterSummaryDto> CreateAsync(
        Guid organizationId,
        CreateServiceEncounterRequest request,
        CancellationToken cancellationToken = default);
}
