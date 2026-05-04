using Caritas.Brigadas.Contracts.FormTemplates;

namespace Caritas.Brigadas.Application.FormTemplates;

public interface IFormTemplateReadRepository
{
    Task<IReadOnlyCollection<FormTemplateSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<FormTemplateSummaryDto?> GetByIdAsync(
        Guid formTemplateId,
        CancellationToken cancellationToken = default);
}
