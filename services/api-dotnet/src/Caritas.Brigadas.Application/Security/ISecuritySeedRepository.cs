using Caritas.Brigadas.Contracts.Security;

namespace Caritas.Brigadas.Application.Security;

public interface ISecuritySeedRepository
{
    Task<SecuritySeedResultDto> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
