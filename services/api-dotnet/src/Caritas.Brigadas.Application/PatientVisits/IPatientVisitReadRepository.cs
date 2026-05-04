using Caritas.Brigadas.Contracts.PatientVisits;

namespace Caritas.Brigadas.Application.PatientVisits;

public interface IPatientVisitReadRepository
{
    Task<IReadOnlyCollection<PatientVisitSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<PatientVisitSummaryDto?> GetByIdAsync(
        Guid visitId,
        CancellationToken cancellationToken = default);
}
