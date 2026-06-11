using Caritas.Brigadas.Application.Patients;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Patients;

public sealed class PatientWriteRepository : IPatientWriteRepository
{
    private static readonly string[] PatientCreateIdempotencyUniqueIndexNames =
    [
        "IX_patients_OrganizationId_IdempotencyKey_UQ",
        "IX_patients_OrganizationId_ClientOperationId_UQ",
        "IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ"
    ];

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
        if (organizationId == Guid.Empty)
        {
            throw new DomainException("Organization id is required.");
        }

        ArgumentNullException.ThrowIfNull(request);

        ValidateCreateRequest(request);

        var existingIdempotentPatient = await FindExistingIdempotentPatientAsync(
            organizationId,
            request,
            cancellationToken);

        if (existingIdempotentPatient is not null)
        {
            return ToSummary(existingIdempotentPatient);
        }

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

        if (request.SourceBrigadeId.HasValue)
        {
            var sourceBrigadeExists = await _dbContext.Brigades
                .AsNoTracking()
                .AnyAsync(
                    brigade =>
                        brigade.Id == request.SourceBrigadeId.Value &&
                        brigade.OrganizationId == organizationId &&
                        !brigade.IsDeleted,
                    cancellationToken);

            if (!sourceBrigadeExists)
            {
                throw new KeyNotFoundException("Source brigade was not found for the organization.");
            }
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

        patient.UpdateOfflineSourceMetadata(
            request.SourceBrigadeId,
            request.LocalPatientId,
            request.ClientOperationId,
            request.IdempotencyKey,
            request.SyncStatus,
            request.DataCaptureSource);

        _dbContext.Patients.Add(patient);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsPatientCreateIdempotencyUniqueViolation(exception))
        {
            var replayedPatient = await FindExistingIdempotentPatientAsync(
                organizationId,
                request,
                cancellationToken);

            if (replayedPatient is not null)
            {
                return ToSummary(replayedPatient);
            }

            throw;
        }

        return ToSummary(patient);
    }




    private static bool IsPatientCreateIdempotencyUniqueViolation(DbUpdateException exception)
    {
        if (exception.InnerException is not SqlException sqlException)
        {
            return false;
        }

        var isUniqueViolation = false;

        foreach (SqlError error in sqlException.Errors)
        {
            if (error.Number is 2601 or 2627)
            {
                isUniqueViolation = true;
                break;
            }
        }

        if (!isUniqueViolation)
        {
            return false;
        }

        foreach (var indexName in PatientCreateIdempotencyUniqueIndexNames)
        {
            if (sqlException.Message.Contains(indexName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<Patient?> FindExistingIdempotentPatientAsync(
        Guid organizationId,
        CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var idempotencyKey = NormalizeOptionalText(request.IdempotencyKey);

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return await _dbContext.Patients
                .AsNoTracking()
                .Where(patient =>
                    patient.OrganizationId == organizationId &&
                    !patient.IsDeleted &&
                    patient.IdempotencyKey == idempotencyKey)
                .OrderBy(patient => patient.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var clientOperationId = NormalizeOptionalText(request.ClientOperationId);

        if (!string.IsNullOrWhiteSpace(clientOperationId))
        {
            return await _dbContext.Patients
                .AsNoTracking()
                .Where(patient =>
                    patient.OrganizationId == organizationId &&
                    !patient.IsDeleted &&
                    patient.ClientOperationId == clientOperationId)
                .OrderBy(patient => patient.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var localPatientId = NormalizeOptionalText(request.LocalPatientId);

        if (!string.IsNullOrWhiteSpace(localPatientId) && request.SourceBrigadeId.HasValue)
        {
            return await _dbContext.Patients
                .AsNoTracking()
                .Where(patient =>
                    patient.OrganizationId == organizationId &&
                    !patient.IsDeleted &&
                    patient.SourceBrigadeId == request.SourceBrigadeId.Value &&
                    patient.LocalPatientId == localPatientId)
                .OrderBy(patient => patient.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return null;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static void ValidateCreateRequest(CreatePatientRequest request)
    {
        var hasIdentitySignal =
            !string.IsNullOrWhiteSpace(request.PatientFolio) ||
            !string.IsNullOrWhiteSpace(request.FirstName) ||
            !string.IsNullOrWhiteSpace(request.PaternalLastName) ||
            !string.IsNullOrWhiteSpace(request.MaternalLastName) ||
            !string.IsNullOrWhiteSpace(request.Curp) ||
            !string.IsNullOrWhiteSpace(request.Phone) ||
            !string.IsNullOrWhiteSpace(request.LocalPatientId) ||
            !string.IsNullOrWhiteSpace(request.ClientOperationId);

        if (!hasIdentitySignal)
        {
            throw new DomainException("At least one patient identity field is required.");
        }

        if (request.IsPartialRecord && string.IsNullOrWhiteSpace(request.PartialRecordReason))
        {
            throw new DomainException("Partial record reason is required when patient record is marked as partial.");
        }

        if (request.SourceBrigadeId.HasValue && request.SourceBrigadeId.Value == Guid.Empty)
        {
            throw new DomainException("Source brigade id cannot be empty.");
        }
    }
    private static PatientSummaryDto ToSummary(Patient patient)
    {
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
            IsActive = patient.IsActive,
            SourceBrigadeId = patient.SourceBrigadeId,
            LocalPatientId = patient.LocalPatientId,
            ClientOperationId = patient.ClientOperationId,
            IdempotencyKey = patient.IdempotencyKey,
            SyncStatus = patient.SyncStatus,
            DataCaptureSource = patient.DataCaptureSource
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