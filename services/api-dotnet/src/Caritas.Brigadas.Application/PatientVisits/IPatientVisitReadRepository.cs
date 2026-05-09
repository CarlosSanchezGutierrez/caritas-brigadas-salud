using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.PatientVisits;

namespace Caritas.Brigadas.Application.PatientVisits;

public interface IPatientVisitReadRepository
{
    Task<PaginatedResponse<PatientVisitSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<PatientVisitSummaryDto?> GetByIdAsync(
        Guid visitId,
        CancellationToken cancellationToken = default);
}