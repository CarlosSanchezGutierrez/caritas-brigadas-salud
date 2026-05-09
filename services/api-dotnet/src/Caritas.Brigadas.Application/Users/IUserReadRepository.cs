using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Users;

namespace Caritas.Brigadas.Application.Users;

public interface IUserReadRepository
{
    Task<PaginatedResponse<UserSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<UserSummaryDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}