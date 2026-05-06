using Caritas.Brigadas.Contracts.FormResponses;

namespace Caritas.Brigadas.Application.FormResponses;

public interface IFormResponseReadRepository
{
    Task<IReadOnlyCollection<FormResponseSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<FormResponseSummaryDto?> GetByIdAsync(
        Guid formResponseId,
        CancellationToken cancellationToken = default);
}
