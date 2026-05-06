using Caritas.Brigadas.Contracts.Audit;

namespace Caritas.Brigadas.Application.Audit;

public interface IAuditLogWriteRepository
{
    Task<AuditLogSummaryDto> CreateAsync(
        CreateAuditLogCommand command,
        CancellationToken cancellationToken = default);
}
