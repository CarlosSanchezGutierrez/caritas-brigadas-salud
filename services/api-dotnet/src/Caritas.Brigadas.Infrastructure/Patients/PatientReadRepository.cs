using Caritas.Brigadas.Application.Patients;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Patients;

public sealed class PatientReadRepository : IPatientReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public PatientReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<PatientSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.NormalizedPageNumber;
        var pageSize = pagination.NormalizedPageSize;

        var query = _dbContext.Patients
            .AsNoTracking()
            .Where(patient =>
                patient.OrganizationId == organizationId &&
                !patient.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(patient => patient.PatientFolio)
            .ThenBy(patient => patient.Id)
            .Skip(pagination.Skip)
            .Take(pageSize)
            .Select(patient => new PatientSummaryDto
            {
                Id = patient.Id,
                OrganizationId = patient.OrganizationId,
                PatientFolio = patient.PatientFolio,
                FirstName = patient.FirstName,
                PaternalLastName = patient.PaternalLastName,
                MaternalLastName = patient.MaternalLastName,
                FullNameNormalized = patient.FullNameNormalized,
                BirthDate = patient.BirthDate,
                ApproximateAge = patient.ApproximateAge,
                Sex = patient.Sex.ToString(),
                Curp = patient.Curp,
                Phone = patient.Phone,
                Municipality = patient.Municipality,
                Colony = patient.Colony,
                Community = patient.Community,
                IsMinor = patient.IsMinor,
                IsMigrant = patient.IsMigrant,
                IsPartialRecord = patient.IsPartialRecord,
                PartialRecordReason = patient.PartialRecordReason,
                Status = patient.Status,
                IsActive = patient.IsActive
            })
            .ToArrayAsync(cancellationToken);

        return new PaginatedResponse<PatientSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PatientSummaryDto?> GetByIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .Where(patient =>
                patient.Id == patientId &&
                !patient.IsDeleted)
            .Select(patient => new PatientSummaryDto
            {
                Id = patient.Id,
                OrganizationId = patient.OrganizationId,
                PatientFolio = patient.PatientFolio,
                FirstName = patient.FirstName,
                PaternalLastName = patient.PaternalLastName,
                MaternalLastName = patient.MaternalLastName,
                FullNameNormalized = patient.FullNameNormalized,
                BirthDate = patient.BirthDate,
                ApproximateAge = patient.ApproximateAge,
                Sex = patient.Sex.ToString(),
                Curp = patient.Curp,
                Phone = patient.Phone,
                Municipality = patient.Municipality,
                Colony = patient.Colony,
                Community = patient.Community,
                IsMinor = patient.IsMinor,
                IsMigrant = patient.IsMigrant,
                IsPartialRecord = patient.IsPartialRecord,
                PartialRecordReason = patient.PartialRecordReason,
                Status = patient.Status,
                IsActive = patient.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
