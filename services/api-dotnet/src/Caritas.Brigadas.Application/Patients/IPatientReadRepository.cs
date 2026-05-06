using Caritas.Brigadas.Contracts.Patients;

namespace Caritas.Brigadas.Application.Patients;

public interface IPatientReadRepository
{
    Task<IReadOnlyCollection<PatientSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<PatientSummaryDto?> GetByIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}
