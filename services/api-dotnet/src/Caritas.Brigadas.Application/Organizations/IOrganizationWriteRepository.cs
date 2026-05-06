using Caritas.Brigadas.Contracts.Organizations;

namespace Caritas.Brigadas.Application.Organizations;

public interface IOrganizationWriteRepository
{
    Task<OrganizationSummaryDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default);
}
