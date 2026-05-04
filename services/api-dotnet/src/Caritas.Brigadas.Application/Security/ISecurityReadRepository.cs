using Caritas.Brigadas.Contracts.Security;

namespace Caritas.Brigadas.Application.Security;

public interface ISecurityReadRepository
{
    Task<IReadOnlyCollection<RoleSummaryDto>> ListRolesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserRoleSummaryDto>> ListUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
