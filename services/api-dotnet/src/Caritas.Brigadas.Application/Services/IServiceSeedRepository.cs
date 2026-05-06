using Caritas.Brigadas.Contracts.Services;

namespace Caritas.Brigadas.Application.Services;

public interface IServiceSeedRepository
{
    Task<ServiceSeedResultDto> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
