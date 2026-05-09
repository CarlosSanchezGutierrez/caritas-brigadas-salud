using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.ConsentDocuments;

namespace Caritas.Brigadas.Application.ConsentDocuments;

public interface IConsentDocumentReadRepository
{
    Task<PaginatedResponse<ConsentDocumentSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<ConsentDocumentSummaryDto?> GetByIdAsync(
        Guid consentDocumentId,
        CancellationToken cancellationToken = default);
}