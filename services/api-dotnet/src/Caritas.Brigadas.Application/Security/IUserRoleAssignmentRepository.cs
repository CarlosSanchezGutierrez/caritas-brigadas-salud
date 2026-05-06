using Caritas.Brigadas.Contracts.Security;

namespace Caritas.Brigadas.Application.Security;

public interface IUserRoleAssignmentRepository
{
    Task<UserRoleSummaryDto> AssignRoleAsync(
        Guid organizationId,
        Guid userId,
        AssignUserRoleRequest request,
        CancellationToken cancellationToken = default);
}
