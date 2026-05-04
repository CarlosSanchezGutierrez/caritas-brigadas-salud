using Caritas.Brigadas.Contracts.PatientVisits;

namespace Caritas.Brigadas.Application.PatientVisits;

public interface IPatientVisitWriteRepository
{
    Task<PatientVisitSummaryDto> CreateAsync(
        Guid organizationId,
        CreatePatientVisitRequest request,
        CancellationToken cancellationToken = default);
}
