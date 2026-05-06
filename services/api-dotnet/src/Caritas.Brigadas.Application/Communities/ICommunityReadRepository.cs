using Caritas.Brigadas.Contracts.Communities;

namespace Caritas.Brigadas.Application.Communities;

public interface ICommunityReadRepository
{
    Task<IReadOnlyCollection<CommunitySummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<CommunitySummaryDto?> GetByIdAsync(
        Guid communityId,
        CancellationToken cancellationToken = default);
}
