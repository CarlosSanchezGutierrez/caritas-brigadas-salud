using Caritas.Brigadas.Contracts.ConsentDocuments;

namespace Caritas.Brigadas.Application.ConsentDocuments;

public interface IConsentDocumentReadRepository
{
    Task<IReadOnlyCollection<ConsentDocumentSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<ConsentDocumentSummaryDto?> GetByIdAsync(
        Guid consentDocumentId,
        CancellationToken cancellationToken = default);
}
