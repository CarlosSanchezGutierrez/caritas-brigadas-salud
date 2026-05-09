using System.Reflection;
using Caritas.Brigadas.Application.PatientVisits;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.PatientVisits;

public sealed class PatientVisitReadRepository : IPatientVisitReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public PatientVisitReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<PatientVisitSummaryDto>> ListByOrganizationAsync(
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

        var query = _dbContext.Set<PatientVisit>()
            .AsNoTracking()
            .Where(visit =>
                EF.Property<Guid>(visit, "OrganizationId") == organizationId &&
                !EF.Property<bool>(visit, "IsDeleted"));

        var totalCount = await query.CountAsync(cancellationToken);

        var visits = await query
            .OrderByDescending(visit =>
                EF.Property<DateTimeOffset?>(visit, "ArrivalTime") ??
                DateTimeOffset.MinValue)
            .ThenByDescending(visit => EF.Property<Guid>(visit, "Id"))
            .Skip(pagination.Skip)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        var items = visits
            .Select(MapToDto)
            .ToArray();

        return new PaginatedResponse<PatientVisitSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PatientVisitSummaryDto?> GetByIdAsync(
        Guid visitId,
        CancellationToken cancellationToken = default)
    {
        if (visitId == Guid.Empty)
        {
            throw new ArgumentException("Patient visit id is required.", nameof(visitId));
        }

        var visit = await _dbContext.Set<PatientVisit>()
            .AsNoTracking()
            .Where(item =>
                EF.Property<Guid>(item, "Id") == visitId &&
                !EF.Property<bool>(item, "IsDeleted"))
            .SingleOrDefaultAsync(cancellationToken);

        return visit is null
            ? null
            : MapToDto(visit);
    }

    private static PatientVisitSummaryDto MapToDto(PatientVisit visit)
    {
        return new PatientVisitSummaryDto
        {
            Id = GetGuidProperty(visit, "Id"),
            OrganizationId = GetGuidProperty(visit, "OrganizationId"),
            VisitFolio = GetStringProperty(visit, "VisitFolio"),
            PatientId = GetGuidProperty(visit, "PatientId"),
            BrigadeId = GetGuidProperty(visit, "BrigadeId"),
            ArrivalTime = GetDateTimeOffsetNullableProperty(visit, "ArrivalTime"),
            RegisteredByUserId =
                GetGuidNullableProperty(visit, "RegisteredByUserId") ??
                GetGuidNullableProperty(visit, "CreatedByUserId"),
            VisitStatus = GetStringProperty(visit, "VisitStatus"),
            CreatedOffline = GetBoolProperty(visit, "CreatedOffline"),
            DeviceId = GetGuidNullableProperty(visit, "DeviceId"),
            SyncStatus = GetStringProperty(visit, "SyncStatus"),
            ClosedAt = GetDateTimeOffsetNullableProperty(visit, "ClosedAt"),
            ClosedByUserId = GetGuidNullableProperty(visit, "ClosedByUserId"),
            IsActive = GetBoolProperty(visit, "IsActive"),
            IsClosed = GetBoolProperty(visit, "IsClosed"),
            NeedsReview = GetBoolProperty(visit, "NeedsReview")
        };
    }

    private static Guid GetGuidProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is Guid guid ? guid : Guid.Empty;
    }

    private static Guid? GetGuidNullableProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is Guid guid ? guid : null;
    }

    private static string GetStringProperty(object instance, string propertyName)
    {
        return GetPropertyValue(instance, propertyName)?.ToString() ?? string.Empty;
    }

    private static bool GetBoolProperty(object instance, string propertyName, bool defaultValue = false)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is bool boolean ? boolean : defaultValue;
    }

    private static DateTimeOffset? GetDateTimeOffsetNullableProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is DateTimeOffset dateTimeOffset ? dateTimeOffset : null;
    }

    private static object? GetPropertyValue(object instance, string propertyName)
    {
        return instance
            .GetType()
            .GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(instance);
    }
}