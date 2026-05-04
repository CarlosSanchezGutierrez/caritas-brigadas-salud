using Caritas.Brigadas.Contracts.Reports;

namespace Caritas.Brigadas.Application.Reports;

public interface IReportReadRepository
{
    Task<OrganizationReportSummaryDto> GetOrganizationSummaryAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
