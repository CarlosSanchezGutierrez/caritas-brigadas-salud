using Caritas.Brigadas.Contracts.Users;

namespace Caritas.Brigadas.Application.Users;

public interface IUserWriteRepository
{
    Task<UserSummaryDto> CreateAsync(
        Guid organizationId,
        CreateUserRequest request,
        CancellationToken cancellationToken = default);
}
