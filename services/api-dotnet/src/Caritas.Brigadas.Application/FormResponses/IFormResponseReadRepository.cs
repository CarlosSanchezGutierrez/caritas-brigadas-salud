using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.FormResponses;

namespace Caritas.Brigadas.Application.FormResponses;

public interface IFormResponseReadRepository
{
    Task<PaginatedResponse<FormResponseSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<FormResponseSummaryDto?> GetByIdAsync(
        Guid formResponseId,
        CancellationToken cancellationToken = default);
}