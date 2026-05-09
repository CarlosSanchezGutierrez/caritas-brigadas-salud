using System.Reflection;
using Caritas.Brigadas.Application.FormResponses;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.FormResponses;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.FormResponses;

public sealed class FormResponseReadRepository : IFormResponseReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public FormResponseReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<FormResponseSummaryDto>> ListByOrganizationAsync(
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

        var query = _dbContext.Set<FormResponse>()
            .AsNoTracking()
            .Where(response =>
                EF.Property<Guid>(response, "OrganizationId") == organizationId &&
                !EF.Property<bool>(response, "IsDeleted"));

        var totalCount = await query.CountAsync(cancellationToken);

        var responses = await query
            .OrderByDescending(response => EF.Property<Guid>(response, "Id"))
            .Skip(pagination.Skip)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        var items = responses
            .Select(MapToDto)
            .ToArray();

        return new PaginatedResponse<FormResponseSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<FormResponseSummaryDto?> GetByIdAsync(
        Guid formResponseId,
        CancellationToken cancellationToken = default)
    {
        if (formResponseId == Guid.Empty)
        {
            throw new ArgumentException("Form response id is required.", nameof(formResponseId));
        }

        var response = await _dbContext.Set<FormResponse>()
            .AsNoTracking()
            .Where(item =>
                EF.Property<Guid>(item, "Id") == formResponseId &&
                !EF.Property<bool>(item, "IsDeleted"))
            .SingleOrDefaultAsync(cancellationToken);

        return response is null
            ? null
            : MapToDto(response);
    }

    private static FormResponseSummaryDto MapToDto(FormResponse response)
    {
        return new FormResponseSummaryDto
        {
            Id = GetGuidProperty(response, "Id"),
            OrganizationId = GetGuidProperty(response, "OrganizationId"),
            EncounterId = GetGuidProperty(response, "EncounterId"),
            FormTemplateId = GetGuidProperty(response, "FormTemplateId"),
            ResponseJson = GetStringProperty(response, "ResponseJson"),
            Status = GetStringProperty(response, "Status"),
            SyncStatus = GetStringProperty(response, "SyncStatus"),
            SubmittedByUserId =
                GetGuidNullableProperty(response, "SubmittedByUserId") ??
                GetGuidNullableProperty(response, "CapturedByUserId") ??
                GetGuidNullableProperty(response, "CreatedByUserId"),
            SubmittedAt =
                GetDateTimeOffsetNullableProperty(response, "SubmittedAt") ??
                GetDateTimeOffsetNullableProperty(response, "CapturedAt") ??
                GetDateTimeOffsetNullableProperty(response, "CreatedAt"),
            CreatedOffline = GetBoolProperty(response, "CreatedOffline"),
            DeviceId = GetGuidNullableProperty(response, "DeviceId"),
            IsDeleted = GetBoolProperty(response, "IsDeleted")
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