using Caritas.Brigadas.Contracts.FormTemplates;

namespace Caritas.Brigadas.Application.FormTemplates;

public interface IFormTemplateSeedRepository
{
    Task<FormTemplateSeedResultDto> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
