using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Patients;

namespace Caritas.Brigadas.Application.Patients;

public interface IPatientReadRepository
{
    Task<PaginatedResponse<PatientSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<PatientSummaryDto?> GetByIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<PatientClinicalRecordDto?> GetClinicalRecordAsync(
        Guid organizationId,
        Guid patientId,
        CancellationToken cancellationToken = default);
}