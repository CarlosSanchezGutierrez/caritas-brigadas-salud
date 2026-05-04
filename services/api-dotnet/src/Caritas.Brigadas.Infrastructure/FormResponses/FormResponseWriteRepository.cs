using System.Reflection;
using Caritas.Brigadas.Application.FormResponses;
using Caritas.Brigadas.Contracts.FormResponses;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.FormResponses;

public sealed class FormResponseWriteRepository : IFormResponseWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public FormResponseWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FormResponseSummaryDto> CreateAsync(
        Guid organizationId,
        CreateFormResponseRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.EncounterId == Guid.Empty)
        {
            throw new DomainException("Encounter id is required.");
        }

        if (request.FormTemplateId == Guid.Empty)
        {
            throw new DomainException("Form template id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ResponseJson))
        {
            throw new DomainException("Response JSON is required.");
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

        var encounterExists = await _dbContext.ServiceEncounters
            .AsNoTracking()
            .AnyAsync(
                encounter =>
                    encounter.Id == request.EncounterId &&
                    encounter.OrganizationId == organizationId &&
                    !encounter.IsDeleted,
                cancellationToken);

        if (!encounterExists)
        {
            throw new KeyNotFoundException("Service encounter was not found in this organization.");
        }

        var formTemplates = await _dbContext.FormTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var formTemplate = formTemplates.SingleOrDefault(template =>
            GetGuidProperty(template, "Id") == request.FormTemplateId &&
            GetGuidProperty(template, "OrganizationId") == organizationId &&
            !GetBoolProperty(template, "IsDeleted"));

        if (formTemplate is null)
        {
            throw new KeyNotFoundException("Form template was not found in this organization.");
        }

        if (request.SubmittedByUserId.HasValue)
        {
            var userExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.SubmittedByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException("Submitted by user was not found in this organization.");
            }
        }

        var existingResponses = await _dbContext.FormResponses
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var duplicateExists = existingResponses.Any(response =>
            GetGuidProperty(response, "EncounterId") == request.EncounterId &&
            GetGuidProperty(response, "FormTemplateId") == request.FormTemplateId &&
            !GetBoolProperty(response, "IsDeleted"));

        if (duplicateExists)
        {
            throw new InvalidOperationException("This encounter already has a response for the selected form template.");
        }

        var now = DateTimeOffset.UtcNow;

        var formResponse = CreateFormResponse(
            organizationId,
            request,
            now);

        _dbContext.FormResponses.Add(formResponse);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(formResponse);
    }

    private static FormResponse CreateFormResponse(
        Guid organizationId,
        CreateFormResponseRequest request,
        DateTimeOffset now)
    {
        var response = (FormResponse)Activator.CreateInstance(
            typeof(FormResponse),
            nonPublic: true)!;

        SetPropertyIfExists(response, "Id", Guid.NewGuid());
        SetPropertyIfExists(response, "OrganizationId", organizationId);
        SetPropertyIfExists(response, "EncounterId", request.EncounterId);
        SetPropertyIfExists(response, "FormTemplateId", request.FormTemplateId);
        SetPropertyIfExists(response, "ResponseJson", request.ResponseJson.Trim());
        SetPropertyIfExists(response, "Status", "Submitted");
        SetPropertyIfExists(response, "SyncStatus", "Synced");
        SetPropertyIfExists(response, "SubmittedByUserId", request.SubmittedByUserId);
        SetPropertyIfExists(response, "CapturedByUserId", request.SubmittedByUserId);
        SetPropertyIfExists(response, "CreatedByUserId", request.SubmittedByUserId);
        SetPropertyIfExists(response, "SubmittedAt", request.SubmittedAt ?? now);
        SetPropertyIfExists(response, "CapturedAt", request.SubmittedAt ?? now);
        SetPropertyIfExists(response, "CreatedOffline", request.CreatedOffline);
        SetPropertyIfExists(response, "DeviceId", request.DeviceId);
        SetPropertyIfExists(response, "CreatedAt", now);
        SetPropertyIfExists(response, "IsDeleted", false);

        return response;
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

    private static void SetPropertyIfExists(
        object instance,
        string propertyName,
        object? value)
    {
        var property = instance
            .GetType()
            .GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null || !property.CanWrite)
        {
            return;
        }

        if (value is null)
        {
            property.SetValue(instance, null);
            return;
        }

        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (targetType.IsEnum && value is string stringValue)
        {
            var parsed = Enum.Parse(targetType, stringValue, ignoreCase: true);
            property.SetValue(instance, parsed);
            return;
        }

        property.SetValue(instance, value);
    }
}
