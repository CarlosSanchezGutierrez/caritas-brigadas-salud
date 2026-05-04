using Caritas.Brigadas.Contracts.Brigades;

namespace Caritas.Brigadas.Application.Brigades;

public interface IBrigadeServiceAssignmentRepository
{
    Task<BrigadeServiceAssignmentDto> AssignAsync(
        Guid brigadeId,
        AssignBrigadeServiceRequest request,
        CancellationToken cancellationToken = default);
}
