using System.Reflection;
using Caritas.Brigadas.Application.FormTemplates;
using Caritas.Brigadas.Contracts.FormTemplates;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.FormTemplates;

public sealed class FormTemplateReadRepository : IFormTemplateReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public FormTemplateReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<FormTemplateSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var templates = await _dbContext.FormTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return templates
            .Where(template =>
                GetGuidProperty(template, "OrganizationId") == organizationId &&
                !GetBoolProperty(template, "IsDeleted"))
            .OrderBy(template => GetStringProperty(template, "FormCode"))
            .ThenBy(template => GetStringProperty(template, "Version"))
            .Select(MapToDto)
            .ToArray();
    }

    public async Task<FormTemplateSummaryDto?> GetByIdAsync(
        Guid formTemplateId,
        CancellationToken cancellationToken = default)
    {
        var templates = await _dbContext.FormTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var template = templates.SingleOrDefault(item =>
            GetGuidProperty(item, "Id") == formTemplateId &&
            !GetBoolProperty(item, "IsDeleted"));

        return template is null
            ? null
            : MapToDto(template);
    }

    private static FormTemplateSummaryDto MapToDto(FormTemplate template)
    {
        return new FormTemplateSummaryDto
        {
            Id = GetGuidProperty(template, "Id"),
            OrganizationId = GetGuidProperty(template, "OrganizationId"),
            ServiceId = GetGuidProperty(template, "ServiceId"),
            FormCode = GetStringProperty(template, "FormCode"),
            Name = GetStringProperty(template, "Name"),
            Version = GetStringProperty(template, "Version"),
            SchemaJson = GetStringProperty(template, "SchemaJson"),
            UiSchemaJson = GetNullableStringProperty(template, "UiSchemaJson"),
            ValidationRulesJson = GetNullableStringProperty(template, "ValidationRulesJson"),
            IsActive = GetBoolProperty(template, "IsActive", defaultValue: true)
        };
    }

    private static Guid GetGuidProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);

        return value is Guid guid
            ? guid
            : Guid.Empty;
    }

    private static string GetStringProperty(object instance, string propertyName)
    {
        return GetPropertyValue(instance, propertyName)?.ToString() ?? string.Empty;
    }

    private static string? GetNullableStringProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName)?.ToString();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
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
