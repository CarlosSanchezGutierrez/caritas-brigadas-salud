using Caritas.Brigadas.Application.Patients;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Patients;

public sealed class PatientWriteRepository : IPatientWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public PatientWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientSummaryDto> CreateAsync(
        Guid organizationId,
        CreatePatientRequest request,
        CancellationToken cancellationToken = default)
    {
        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Id == organizationId &&
                    !organization.IsDeleted,
                cancellationToken);

        if (!organizationExists)
        {
            throw new KeyNotFoundException("Organization was not found.");
        }

        var patientFolio = string.IsNullOrWhiteSpace(request.PatientFolio)
            ? GeneratePatientFolio()
            : request.PatientFolio.Trim();

        var folioExists = await _dbContext.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.OrganizationId == organizationId &&
                    patient.PatientFolio == patientFolio.ToUpperInvariant() &&
                    !patient.IsDeleted,
                cancellationToken);

        if (folioExists)
        {
            throw new InvalidOperationException("A patient with the same folio already exists.");
        }

        var sex = ParseSex(request.Sex);

        var patient = new Patient(
            Guid.NewGuid(),
            organizationId,
            patientFolio,
            request.FirstName,
            request.PaternalLastName,
            request.MaternalLastName,
            request.BirthDate,
            request.ApproximateAge,
            sex);

        patient.UpdateSensitiveIdentifiers(
            request.Curp,
            request.Phone);

        patient.UpdateLocation(
            request.AddressLine,
            request.Municipality,
            request.Colony,
            request.Community);

        if (request.IsMigrant)
        {
            patient.MarkAsMigrant();
        }

        if (request.IsPartialRecord)
        {
            if (string.IsNullOrWhiteSpace(request.PartialRecordReason))
            {
                throw new DomainException("Partial record reason is required when patient record is marked as partial.");
            }

            patient.MarkAsPartialRecord(request.PartialRecordReason);
        }

        patient.UpdateAdminNotes(request.NotesAdmin);

        _dbContext.Patients.Add(patient);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PatientSummaryDto
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
        };
    }

    private static string GeneratePatientFolio()
    {
        return $"PAT-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static Sex ParseSex(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Sex.NotSpecified;
        }

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "male" or "masculino" or "m" => Sex.Male,
            "female" or "femenino" or "f" => Sex.Female,
            _ => Sex.NotSpecified
        };
    }
}
