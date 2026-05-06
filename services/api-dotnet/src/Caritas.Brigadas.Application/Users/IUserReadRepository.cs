using Caritas.Brigadas.Contracts.Users;

namespace Caritas.Brigadas.Application.Users;

public interface IUserReadRepository
{
    Task<IReadOnlyCollection<UserSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<UserSummaryDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
