using Caritas.Brigadas.Contracts.MobileUnits;

namespace Caritas.Brigadas.Application.MobileUnits;

public interface IMobileUnitReadRepository
{
    Task<IReadOnlyCollection<MobileUnitSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<MobileUnitSummaryDto?> GetByIdAsync(
        Guid mobileUnitId,
        CancellationToken cancellationToken = default);
}
