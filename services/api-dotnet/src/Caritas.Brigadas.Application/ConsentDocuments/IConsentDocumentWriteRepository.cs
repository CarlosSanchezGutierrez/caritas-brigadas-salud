using Caritas.Brigadas.Contracts.ConsentDocuments;

namespace Caritas.Brigadas.Application.ConsentDocuments;

public interface IConsentDocumentWriteRepository
{
    Task<ConsentDocumentSummaryDto> CreateAsync(
        Guid organizationId,
        CreateConsentDocumentRequest request,
        CancellationToken cancellationToken = default);
}
