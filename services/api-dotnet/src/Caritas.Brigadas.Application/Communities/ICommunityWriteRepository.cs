using Caritas.Brigadas.Contracts.Communities;

namespace Caritas.Brigadas.Application.Communities;

public interface ICommunityWriteRepository
{
    Task<CommunitySummaryDto> CreateAsync(
        Guid organizationId,
        CreateCommunityRequest request,
        CancellationToken cancellationToken = default);
}
