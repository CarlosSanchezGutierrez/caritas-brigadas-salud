using Caritas.Brigadas.Contracts.Patients;

namespace Caritas.Brigadas.Application.Patients;

public interface IPatientWriteRepository
{
    Task<PatientSummaryDto> CreateAsync(
        Guid organizationId,
        CreatePatientRequest request,
        CancellationToken cancellationToken = default);
}
