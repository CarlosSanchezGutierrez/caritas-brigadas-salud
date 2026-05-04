using System.Reflection;
using Caritas.Brigadas.Application.FormResponses;
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

    public async Task<IReadOnlyCollection<FormResponseSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var responses = await _dbContext.FormResponses
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return responses
            .Where(response =>
                GetGuidProperty(response, "OrganizationId") == organizationId &&
                !GetBoolProperty(response, "IsDeleted"))
            .OrderByDescending(response =>
                GetDateTimeOffsetNullableProperty(response, "SubmittedAt") ??
                GetDateTimeOffsetNullableProperty(response, "CapturedAt") ??
                GetDateTimeOffsetNullableProperty(response, "CreatedAt"))
            .Select(MapToDto)
            .ToArray();
    }

    public async Task<FormResponseSummaryDto?> GetByIdAsync(
        Guid formResponseId,
        CancellationToken cancellationToken = default)
    {
        var responses = await _dbContext.FormResponses
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var response = responses.SingleOrDefault(item =>
            GetGuidProperty(item, "Id") == formResponseId &&
            !GetBoolProperty(item, "IsDeleted"));

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

        return value is Guid guid
            ? guid
            : Guid.Empty;
    }

    private static Guid? GetGuidNullableProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);

        return value is Guid guid
            ? guid
            : null;
    }

    private static string GetStringProperty(object instance, string propertyName)
    {
        return GetPropertyValue(instance, propertyName)?.ToString() ?? string.Empty;
    }

    private static bool GetBoolProperty(
        object instance,
        string propertyName,
        bool defaultValue = false)
    {
        var value = GetPropertyValue(instance, propertyName);

        return value is bool boolean
            ? boolean
            : defaultValue;
    }

    private static DateTimeOffset? GetDateTimeOffsetNullableProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);

        return value is DateTimeOffset dateTimeOffset
            ? dateTimeOffset
            : null;
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
