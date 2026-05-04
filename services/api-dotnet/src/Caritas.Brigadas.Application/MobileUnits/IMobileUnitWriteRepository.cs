using Caritas.Brigadas.Contracts.MobileUnits;

namespace Caritas.Brigadas.Application.MobileUnits;

public interface IMobileUnitWriteRepository
{
    Task<MobileUnitSummaryDto> CreateAsync(
        Guid organizationId,
        CreateMobileUnitRequest request,
        CancellationToken cancellationToken = default);
}
