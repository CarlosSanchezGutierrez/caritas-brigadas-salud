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
                IsActive = patient.IsActive,
                SourceBrigadeId = patient.SourceBrigadeId,
                LocalPatientId = patient.LocalPatientId,
                ClientOperationId = patient.ClientOperationId,
                IdempotencyKey = patient.IdempotencyKey,
                SyncStatus = patient.SyncStatus,
                DataCaptureSource = patient.DataCaptureSource
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


    public async Task<PatientClinicalRecordDto?> GetClinicalRecordAsync(
        Guid organizationId,
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        var patient = await _dbContext.Patients
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                entity.Id == patientId &&
                !entity.IsDeleted)
            .Select(entity => new PatientSummaryDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                PatientFolio = entity.PatientFolio,
                FirstName = entity.FirstName,
                PaternalLastName = entity.PaternalLastName,
                MaternalLastName = entity.MaternalLastName,
                FullNameNormalized = entity.FullNameNormalized,
                BirthDate = entity.BirthDate,
                ApproximateAge = entity.ApproximateAge,
                Sex = entity.Sex.ToString(),
                Curp = entity.Curp,
                Phone = entity.Phone,
                Municipality = entity.Municipality,
                Colony = entity.Colony,
                Community = entity.Community,
                IsMinor = entity.IsMinor,
                IsMigrant = entity.IsMigrant,
                IsPartialRecord = entity.IsPartialRecord,
                PartialRecordReason = entity.PartialRecordReason,
                Status = entity.Status.ToString(),
                IsActive = entity.IsActive,
                SourceBrigadeId = entity.SourceBrigadeId,
                LocalPatientId = entity.LocalPatientId,
                ClientOperationId = entity.ClientOperationId,
                IdempotencyKey = entity.IdempotencyKey,
                SyncStatus = entity.SyncStatus,
                DataCaptureSource = entity.DataCaptureSource
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
        {
            return null;
        }

        var visits = await _dbContext.PatientVisits
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                entity.PatientId == patientId &&
                !entity.IsDeleted)
            .OrderByDescending(entity => entity.ArrivalTime)
            .ThenByDescending(entity => entity.Id)
            .Select(entity => new PatientClinicalRecordVisitDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                VisitFolio = entity.VisitFolio,
                PatientId = entity.PatientId,
                BrigadeId = entity.BrigadeId,
                ArrivalTime = entity.ArrivalTime,
                RegisteredByUserId = entity.RegisteredByUserId,
                VisitStatus = entity.VisitStatus.ToString(),
                CreatedOffline = entity.CreatedOffline,
                DeviceId = entity.DeviceId,
                SyncStatus = entity.SyncStatus.ToString(),
                ClosedAt = entity.ClosedAt,
                ClosedByUserId = entity.ClosedByUserId,
                IsActive = entity.IsActive,
                IsClosed = entity.IsClosed,
                NeedsReview = entity.NeedsReview
            })
            .ToArrayAsync(cancellationToken);

        var encounters = await _dbContext.ServiceEncounters
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                entity.PatientId == patientId &&
                !entity.IsDeleted)
            .OrderByDescending(entity => entity.StartedAt)
            .ThenByDescending(entity => entity.Id)
            .Select(entity => new PatientClinicalRecordEncounterDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                EncounterFolio = entity.EncounterFolio,
                VisitId = entity.VisitId,
                PatientId = entity.PatientId,
                BrigadeId = entity.BrigadeId,
                ServiceId = entity.ServiceId,
                ProviderUserId = entity.ProviderUserId,
                StartedAt = entity.StartedAt,
                CompletedAt = entity.EndedAt,
                Status = entity.Status.ToString(),
                CreatedOffline = entity.CreatedOffline,
                DeviceId = entity.DeviceId,
                SyncStatus = entity.SyncStatus.ToString(),
                IsActive = entity.IsActive,
                IsCompleted = entity.IsCompleted,
                NeedsReview = entity.NeedsReview
            })
            .ToArrayAsync(cancellationToken);

        var vitalSigns = await _dbContext.VitalSignsRecords
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                entity.PatientId == patientId &&
                !entity.IsDeleted)
            .OrderByDescending(entity => entity.MeasuredAt)
            .ThenByDescending(entity => entity.Id)
            .Select(entity => new PatientClinicalRecordVitalSignsDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                PatientId = entity.PatientId,
                VisitId = entity.VisitId,
                EncounterId = entity.EncounterId,
                MeasuredByUserId = entity.MeasuredByUserId,
                MeasuredAt = entity.MeasuredAt,
                SystolicBloodPressureMmHg = entity.SystolicBloodPressureMmHg,
                DiastolicBloodPressureMmHg = entity.DiastolicBloodPressureMmHg,
                HeartRateBpm = entity.HeartRateBpm,
                RespiratoryRatePerMinute = entity.RespiratoryRatePerMinute,
                TemperatureCelsius = entity.TemperatureCelsius,
                OxygenSaturationPercent = entity.OxygenSaturationPercent,
                WeightKg = entity.WeightKg,
                HeightCm = entity.HeightCm,
                GlucoseMgDl = entity.GlucoseMgDl,
                Source = entity.Source,
                Notes = entity.Notes,
                CreatedOffline = entity.CreatedOffline,
                DeviceId = entity.DeviceId,
                SyncStatus = entity.SyncStatus.ToString()
            })
            .ToArrayAsync(cancellationToken);

        var formResponses = await _dbContext.FormResponses
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                !entity.IsDeleted &&
                _dbContext.ServiceEncounters.Any(encounter =>
                    encounter.OrganizationId == organizationId &&
                    encounter.PatientId == patientId &&
                    encounter.Id == entity.EncounterId &&
                    !encounter.IsDeleted))
            .OrderByDescending(entity => entity.CompletedAt)
            .ThenByDescending(entity => entity.Id)
            .Select(entity => new PatientClinicalRecordFormResponseDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                EncounterId = entity.EncounterId,
                FormTemplateId = entity.FormTemplateId,
                ResponseHash = entity.ResponseHash,
                CompletedByUserId = entity.CompletedByUserId,
                CompletedAt = entity.CompletedAt,
                Status = entity.Status,
                CreatedOffline = entity.CreatedOffline,
                DeviceId = entity.DeviceId,
                SubmittedAt = entity.SubmittedAt,
                CapturedAt = entity.CapturedAt,
                SyncStatus = entity.SyncStatus.ToString()
            })
            .ToArrayAsync(cancellationToken);

        var consentDocuments = await _dbContext.ConsentDocuments
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                entity.PatientId == patientId &&
                !entity.IsDeleted)
            .OrderByDescending(entity => entity.SignedAt)
            .ThenByDescending(entity => entity.Id)
            .Select(entity => new PatientClinicalRecordConsentDocumentDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                PatientId = entity.PatientId,
                VisitId = entity.VisitId,
                ConsentType = entity.ConsentType,
                DocumentVersion = entity.DocumentVersion,
                HasSignature = entity.SignatureDataUrl != null && entity.SignatureDataUrl != string.Empty,
                GuardianFullName = entity.GuardianFullName,
                GuardianRelationship = entity.GuardianRelationship,
                SignedByUserId = entity.SignedByUserId,
                SignedAt = entity.SignedAt,
                CreatedOffline = entity.CreatedOffline,
                DeviceId = entity.DeviceId,
                SyncStatus = entity.SyncStatus,
                CreatedAt = entity.CreatedAt
            })
            .ToArrayAsync(cancellationToken);

        var medicalReferrals = await _dbContext.MedicalReferrals
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                entity.PatientId == patientId &&
                !entity.IsDeleted)
            .OrderByDescending(entity => entity.CreatedAt)
            .ThenByDescending(entity => entity.Id)
            .Select(entity => new PatientClinicalRecordMedicalReferralDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                EncounterId = entity.EncounterId,
                PatientId = entity.PatientId,
                ReferralFolio = entity.ReferralFolio,
                DestinationInstitution = entity.DestinationInstitution,
                ReferralReason = entity.ReferralReason,
                Priority = entity.Priority,
                ReferredByUserId = entity.ReferredByUserId,
                ProviderSignatureId = entity.ProviderSignatureId,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            })
            .ToArrayAsync(cancellationToken);

        var medicationDeliveries = await _dbContext.MedicationDeliveries
            .AsNoTracking()
            .Where(entity =>
                entity.OrganizationId == organizationId &&
                entity.PatientId == patientId &&
                !entity.IsDeleted)
            .OrderByDescending(entity => entity.CreatedAt)
            .ThenByDescending(entity => entity.Id)
            .Select(entity => new PatientClinicalRecordMedicationDeliveryDto
            {
                Id = entity.Id,
                OrganizationId = entity.OrganizationId,
                EncounterId = entity.EncounterId,
                PatientId = entity.PatientId,
                MedicationName = entity.MedicationName,
                Presentation = entity.Presentation,
                Quantity = entity.Quantity,
                LotNumber = entity.LotNumber,
                ExpirationDate = entity.ExpirationDate,
                Instructions = entity.Instructions,
                DeliveredByUserId = entity.DeliveredByUserId,
                ReceivedByName = entity.ReceivedByName,
                SignatureId = entity.SignatureId,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            })
            .ToArrayAsync(cancellationToken);
        return new PatientClinicalRecordDto
        {
            OrganizationId = organizationId,
            PatientId = patientId,
            Patient = patient,
            Visits = visits,
            Encounters = encounters,
            VitalSigns = vitalSigns,
            FormResponses = formResponses,
            ConsentDocuments = consentDocuments,
            MedicalReferrals = medicalReferrals,
            MedicationDeliveries = medicationDeliveries,
            Summary = new PatientClinicalRecordSummaryDto
            {
                VisitCount = visits.Length,
                EncounterCount = encounters.Length,
                VitalSignsCount = vitalSigns.Length,
                FormResponseCount = formResponses.Length,
                ConsentDocumentCount = consentDocuments.Length,
                MedicalReferralCount = medicalReferrals.Length,
                MedicationDeliveryCount = medicationDeliveries.Length,
                FirstVisitAt = visits
                    .Where(visit => visit.ArrivalTime.HasValue)
                    .OrderBy(visit => visit.ArrivalTime)
                    .Select(visit => visit.ArrivalTime)
                    .FirstOrDefault(),
                LastVisitAt = visits
                    .Where(visit => visit.ArrivalTime.HasValue)
                    .OrderByDescending(visit => visit.ArrivalTime)
                    .Select(visit => visit.ArrivalTime)
                    .FirstOrDefault(),
                LastVitalSignsMeasuredAt = vitalSigns
                    .OrderByDescending(record => record.MeasuredAt)
                    .Select(record => (DateTimeOffset?)record.MeasuredAt)
                    .FirstOrDefault()
            }
        };
    }
    public async Task<PatientSummaryDto?> GetByIdAsync(
        Guid organizationId,
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        return await _dbContext.Patients
            .AsNoTracking()
            .Where(patient =>
                patient.OrganizationId == organizationId &&
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
                IsActive = patient.IsActive,
                SourceBrigadeId = patient.SourceBrigadeId,
                LocalPatientId = patient.LocalPatientId,
                ClientOperationId = patient.ClientOperationId,
                IdempotencyKey = patient.IdempotencyKey,
                SyncStatus = patient.SyncStatus,
                DataCaptureSource = patient.DataCaptureSource
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
