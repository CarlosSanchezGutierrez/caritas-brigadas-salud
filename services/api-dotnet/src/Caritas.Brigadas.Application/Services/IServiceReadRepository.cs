using Caritas.Brigadas.Contracts.Services;

namespace Caritas.Brigadas.Application.Services;

public interface IServiceReadRepository
{
    Task<IReadOnlyCollection<ServiceSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
