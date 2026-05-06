using Caritas.Brigadas.Contracts.FormResponses;

namespace Caritas.Brigadas.Application.FormResponses;

public interface IFormResponseWriteRepository
{
    Task<FormResponseSummaryDto> CreateAsync(
        Guid organizationId,
        CreateFormResponseRequest request,
        CancellationToken cancellationToken = default);
}
