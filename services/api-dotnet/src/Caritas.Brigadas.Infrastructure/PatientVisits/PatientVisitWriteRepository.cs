using Caritas.Brigadas.Application.PatientVisits;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.PatientVisits;

public sealed class PatientVisitWriteRepository : IPatientVisitWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public PatientVisitWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientVisitSummaryDto> CreateAsync(
        Guid organizationId,
        CreatePatientVisitRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new DomainException("Patient id is required.");
        }

        if (request.BrigadeId == Guid.Empty)
        {
            throw new DomainException("Brigade id is required.");
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

        var patientExists = await _dbContext.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.Id == request.PatientId &&
                    patient.OrganizationId == organizationId &&
                    !patient.IsDeleted,
                cancellationToken);

        if (!patientExists)
        {
            throw new KeyNotFoundException("Patient was not found in this organization.");
        }

        var brigadeExists = await _dbContext.Brigades
            .AsNoTracking()
            .AnyAsync(
                brigade =>
                    brigade.Id == request.BrigadeId &&
                    brigade.OrganizationId == organizationId &&
                    !brigade.IsDeleted,
                cancellationToken);

        if (!brigadeExists)
        {
            throw new KeyNotFoundException("Brigade was not found in this organization.");
        }

        if (request.RegisteredByUserId.HasValue)
        {
            var registeredByExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.RegisteredByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!registeredByExists)
            {
                throw new KeyNotFoundException("Registered by user was not found in this organization.");
            }
        }

        var visitFolio = string.IsNullOrWhiteSpace(request.VisitFolio)
            ? GenerateVisitFolio()
            : request.VisitFolio.Trim();

        var normalizedVisitFolio = visitFolio.ToUpperInvariant();

        var folioExists = await _dbContext.PatientVisits
            .AsNoTracking()
            .AnyAsync(
                visit =>
                    visit.OrganizationId == organizationId &&
                    visit.VisitFolio == normalizedVisitFolio &&
                    !visit.IsDeleted,
                cancellationToken);

        if (folioExists)
        {
            throw new InvalidOperationException("A patient visit with the same folio already exists.");
        }

        var visit = new PatientVisit(
            Guid.NewGuid(),
            organizationId,
            normalizedVisitFolio,
            request.PatientId,
            request.BrigadeId,
            request.ArrivalTime ?? DateTimeOffset.UtcNow,
            request.RegisteredByUserId,
            request.CreatedOffline,
            request.DeviceId);

        _dbContext.PatientVisits.Add(visit);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PatientVisitSummaryDto
        {
            Id = visit.Id,
            OrganizationId = visit.OrganizationId,
            VisitFolio = visit.VisitFolio,
            PatientId = visit.PatientId,
            BrigadeId = visit.BrigadeId,
            ArrivalTime = visit.ArrivalTime,
            RegisteredByUserId = visit.RegisteredByUserId,
            VisitStatus = visit.VisitStatus.ToString(),
            CreatedOffline = visit.CreatedOffline,
            DeviceId = visit.DeviceId,
            SyncStatus = visit.SyncStatus.ToString(),
            ClosedAt = visit.ClosedAt,
            ClosedByUserId = visit.ClosedByUserId,
            IsActive = visit.IsActive,
            IsClosed = visit.IsClosed,
            NeedsReview = visit.NeedsReview
        };
    }

    private static string GenerateVisitFolio()
    {
        return $"VISIT-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}";
    }
}
