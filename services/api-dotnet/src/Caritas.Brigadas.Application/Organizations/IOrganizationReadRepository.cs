using Caritas.Brigadas.Contracts.Organizations;

namespace Caritas.Brigadas.Application.Organizations;

public interface IOrganizationReadRepository
{
    Task<IReadOnlyCollection<OrganizationSummaryDto>> ListAsync(
        CancellationToken cancellationToken = default);

    Task<OrganizationSummaryDto?> GetByIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
