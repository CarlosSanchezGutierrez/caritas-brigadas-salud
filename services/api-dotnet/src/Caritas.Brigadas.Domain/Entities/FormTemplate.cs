using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class FormTemplate : AuditableEntity
{
    private const int MaxFormCodeLength = 100;
    private const int MaxNameLength = 200;
    private const int MaxVersionLength = 50;

    private FormTemplate()
    {
        FormCode = string.Empty;
        Name = string.Empty;
        Version = string.Empty;
        SchemaJson = string.Empty;
        IsActive = true;
    }

    public FormTemplate(
        Guid id,
        Guid organizationId,
        Guid serviceId,
        string formCode,
        string name,
        string version,
        string schemaJson,
        string? uiSchemaJson = null,
        string? validationRulesJson = null,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? effectiveTo = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        ServiceId = RequireGuid(serviceId, nameof(serviceId));
        FormCode = NormalizeRequired(formCode, nameof(formCode), MaxFormCodeLength).ToUpperInvariant();
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        Version = NormalizeRequired(version, nameof(version), MaxVersionLength);
        SchemaJson = NormalizeJson(schemaJson, nameof(schemaJson));
        UiSchemaJson = NormalizeOptionalJson(uiSchemaJson);
        ValidationRulesJson = NormalizeOptionalJson(validationRulesJson);
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        IsActive = true;

        ValidateEffectiveDates();
    }

    public Guid OrganizationId { get; private set; }

    public Guid ServiceId { get; private set; }

    public string FormCode { get; private set; }

    public string Name { get; private set; }

    public string Version { get; private set; }

    public string SchemaJson { get; private set; }

    public string? UiSchemaJson { get; private set; }

    public string? ValidationRulesJson { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset? EffectiveFrom { get; private set; }

    public DateTimeOffset? EffectiveTo { get; private set; }

    public void UpdateMetadata(string name, DateTimeOffset? effectiveFrom, DateTimeOffset? effectiveTo)
    {
        Name = NormalizeRequired(name, nameof(name), MaxNameLength);
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;

        ValidateEffectiveDates();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void ValidateEffectiveDates()
    {
        if (EffectiveFrom.HasValue &&
            EffectiveTo.HasValue &&
            EffectiveTo.Value <= EffectiveFrom.Value)
        {
            throw new DomainException("Effective end date must be after effective start date.");
        }
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string NormalizeJson(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptionalJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
