using Caritas.Brigadas.Application.ServiceEncounters;
using Caritas.Brigadas.Contracts.ServiceEncounters;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.ServiceEncounters;

public sealed class ServiceEncounterWriteRepository : IServiceEncounterWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public ServiceEncounterWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceEncounterSummaryDto> CreateAsync(
        Guid organizationId,
        CreateServiceEncounterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.VisitId == Guid.Empty)
        {
            throw new DomainException("Visit id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ServiceCode))
        {
            throw new DomainException("Service code is required.");
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

        var visit = await _dbContext.PatientVisits
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.VisitId &&
                    item.OrganizationId == organizationId &&
                    !item.IsDeleted,
                cancellationToken);

        if (visit is null)
        {
            throw new KeyNotFoundException("Patient visit was not found in this organization.");
        }

        if (visit.IsClosed)
        {
            throw new InvalidOperationException("Closed patient visits cannot receive new service encounters.");
        }

        var serviceCode = request.ServiceCode.Trim().ToUpperInvariant();

        var service = await _dbContext.Services
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.OrganizationId == organizationId &&
                    item.Code == serviceCode &&
                    !item.IsDeleted,
                cancellationToken);

        if (service is null)
        {
            throw new KeyNotFoundException("Service was not found in this organization.");
        }

        if (!service.IsActive)
        {
            throw new InvalidOperationException("Inactive services cannot be used for service encounters.");
        }

        var serviceAssignedToBrigade = await _dbContext.BrigadeServices
            .AsNoTracking()
            .AnyAsync(
                assignment =>
                    assignment.BrigadeId == visit.BrigadeId &&
                    assignment.ServiceId == service.Id &&
                    assignment.IsAvailable &&
                    !assignment.IsDeleted,
                cancellationToken);

        if (!serviceAssignedToBrigade)
        {
            throw new InvalidOperationException("Service is not assigned as available for this brigade.");
        }

        if (request.ProviderUserId.HasValue)
        {
            var providerExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.ProviderUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!providerExists)
            {
                throw new KeyNotFoundException("Provider user was not found in this organization.");
            }
        }

        var encounterFolio = string.IsNullOrWhiteSpace(request.EncounterFolio)
            ? GenerateEncounterFolio()
            : request.EncounterFolio.Trim();

        var normalizedEncounterFolio = encounterFolio.ToUpperInvariant();

        var folioExists = await _dbContext.ServiceEncounters
            .AsNoTracking()
            .AnyAsync(
                encounter =>
                    encounter.OrganizationId == organizationId &&
                    encounter.EncounterFolio == normalizedEncounterFolio &&
                    !encounter.IsDeleted,
                cancellationToken);

        if (folioExists)
        {
            throw new InvalidOperationException("A service encounter with the same folio already exists.");
        }

        var duplicateActiveEncounter = await _dbContext.ServiceEncounters
            .AsNoTracking()
            .AnyAsync(
                encounter =>
                    encounter.VisitId == request.VisitId &&
                    encounter.ServiceId == service.Id &&
                    !encounter.IsDeleted &&
                    encounter.IsActive,
                cancellationToken);

        if (duplicateActiveEncounter)
        {
            throw new InvalidOperationException("This visit already has an active encounter for the selected service.");
        }

        var encounter = new ServiceEncounter(
            Guid.NewGuid(),
            organizationId,
            normalizedEncounterFolio,
            request.VisitId,
            visit.PatientId,
            visit.BrigadeId,
            service.Id,
            request.ProviderUserId,
            request.StartedAt ?? DateTimeOffset.UtcNow,
            request.CreatedOffline,
            request.DeviceId);

        _dbContext.ServiceEncounters.Add(encounter);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ServiceEncounterSummaryDto
        {
            Id = encounter.Id,
            OrganizationId = encounter.OrganizationId,
            EncounterFolio = encounter.EncounterFolio,
            VisitId = encounter.VisitId,
            PatientId = visit.PatientId,
            BrigadeId = visit.BrigadeId,
            ServiceId = encounter.ServiceId,
            ServiceCode = service.Code,
            ServiceName = service.Name,
            ProviderUserId = encounter.ProviderUserId,
            StartedAt = encounter.StartedAt,
            CompletedAt = null,
            Status = encounter.Status.ToString(),
            CreatedOffline = encounter.CreatedOffline,
            DeviceId = encounter.DeviceId,
            SyncStatus = encounter.SyncStatus.ToString(),
            IsActive = encounter.IsActive,
            IsCompleted = encounter.IsCompleted,
            NeedsReview = encounter.NeedsReview
        };
    }

    private static string GenerateEncounterFolio()
    {
        return $"ENC-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}";
    }
}
