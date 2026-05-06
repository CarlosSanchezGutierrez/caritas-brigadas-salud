using System.Reflection;
using Caritas.Brigadas.Application.FormTemplates;
using Caritas.Brigadas.Contracts.FormTemplates;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.FormTemplates;

public sealed class FormTemplateSeedRepository : IFormTemplateSeedRepository
{
    private readonly CaritasDbContext _dbContext;

    public FormTemplateSeedRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FormTemplateSeedResultDto> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
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

        var services = await _dbContext.Services
            .AsNoTracking()
            .Where(service =>
                service.OrganizationId == organizationId &&
                !service.IsDeleted)
            .ToListAsync(cancellationToken);

        var existingTemplates = await _dbContext.FormTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var definitions = GetDefinitions();
        var created = 0;

        foreach (var definition in definitions)
        {
            var service = services.SingleOrDefault(item =>
                string.Equals(item.Code, definition.ServiceCode, StringComparison.OrdinalIgnoreCase));

            if (service is null)
            {
                throw new KeyNotFoundException($"Service '{definition.ServiceCode}' was not found. Seed services first.");
            }

            var duplicateExists = existingTemplates.Any(template =>
                GetGuidProperty(template, "OrganizationId") == organizationId &&
                GetGuidProperty(template, "ServiceId") == service.Id &&
                string.Equals(GetStringProperty(template, "FormCode"), definition.FormCode, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(GetStringProperty(template, "Version"), definition.Version, StringComparison.OrdinalIgnoreCase) &&
                !GetBoolProperty(template, "IsDeleted"));

            if (duplicateExists)
            {
                continue;
            }

            var template = CreateFormTemplate(
                organizationId,
                service.Id,
                definition);

            _dbContext.FormTemplates.Add(template);
            created++;
        }

        if (created > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var formCodes = await _dbContext.FormTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new FormTemplateSeedResultDto
        {
            OrganizationId = organizationId,
            FormTemplatesCreated = created,
            FormCodes = formCodes
                .Where(template =>
                    GetGuidProperty(template, "OrganizationId") == organizationId &&
                    !GetBoolProperty(template, "IsDeleted"))
                .Select(template => GetStringProperty(template, "FormCode"))
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(code => code)
                .ToArray()
        };
    }

    private static FormTemplate CreateFormTemplate(
        Guid organizationId,
        Guid serviceId,
        FormTemplateDefinition definition)
    {
        var template = (FormTemplate)Activator.CreateInstance(
            typeof(FormTemplate),
            nonPublic: true)!;

        SetPropertyIfExists(template, "Id", Guid.NewGuid());
        SetPropertyIfExists(template, "OrganizationId", organizationId);
        SetPropertyIfExists(template, "ServiceId", serviceId);
        SetPropertyIfExists(template, "FormCode", definition.FormCode);
        SetPropertyIfExists(template, "Name", definition.Name);
        SetPropertyIfExists(template, "Version", definition.Version);
        SetPropertyIfExists(template, "SchemaJson", definition.SchemaJson);
        SetPropertyIfExists(template, "UiSchemaJson", definition.UiSchemaJson);
        SetPropertyIfExists(template, "ValidationRulesJson", definition.ValidationRulesJson);
        SetPropertyIfExists(template, "IsActive", true);
        SetPropertyIfExists(template, "IsDeleted", false);
        SetPropertyIfExists(template, "CreatedAt", DateTimeOffset.UtcNow);

        return template;
    }

    private static IReadOnlyCollection<FormTemplateDefinition> GetDefinitions()
    {
        return new[]
        {
            new FormTemplateDefinition(
                "GENERAL_MEDICINE",
                "GENERAL_MEDICINE_V1",
                "Consulta médica general",
                "1.0.0",
                """
                {
                  "type": "object",
                  "title": "Consulta médica general",
                  "required": ["chiefComplaint", "bloodPressure", "recommendations"],
                  "properties": {
                    "chiefComplaint": { "type": "string", "title": "Motivo de consulta" },
                    "bloodPressure": { "type": "string", "title": "Presión arterial" },
                    "temperatureCelsius": { "type": ["number", "null"], "title": "Temperatura °C" },
                    "weightKg": { "type": ["number", "null"], "title": "Peso kg" },
                    "clinicalNotes": { "type": "string", "title": "Notas clínicas" },
                    "recommendations": { "type": "string", "title": "Recomendaciones" },
                    "requiresFollowUp": { "type": "boolean", "title": "Requiere seguimiento" },
                    "requiresReferral": { "type": "boolean", "title": "Requiere referencia" }
                  }
                }
                """,
                """
                {
                  "sections": [
                    { "title": "Consulta", "fields": ["chiefComplaint", "bloodPressure", "temperatureCelsius", "weightKg"] },
                    { "title": "Notas y cierre", "fields": ["clinicalNotes", "recommendations", "requiresFollowUp", "requiresReferral"] }
                  ]
                }
                """,
                """
                {
                  "requiresSensitiveHandling": true,
                  "allowOfflineCapture": true
                }
                """),

            new FormTemplateDefinition(
                "DENTISTRY",
                "DENTISTRY_V1",
                "Odontología",
                "1.0.0",
                """
                {
                  "type": "object",
                  "title": "Odontología",
                  "required": ["reason", "findings", "recommendations"],
                  "properties": {
                    "reason": { "type": "string", "title": "Motivo de atención" },
                    "findings": { "type": "string", "title": "Hallazgos" },
                    "procedurePerformed": { "type": ["string", "null"], "title": "Procedimiento realizado" },
                    "recommendations": { "type": "string", "title": "Recomendaciones" },
                    "requiresReferral": { "type": "boolean", "title": "Requiere referencia" }
                  }
                }
                """,
                """
                {
                  "sections": [
                    { "title": "Valoración dental", "fields": ["reason", "findings", "procedurePerformed"] },
                    { "title": "Cierre", "fields": ["recommendations", "requiresReferral"] }
                  ]
                }
                """,
                """
                {
                  "requiresSensitiveHandling": true,
                  "allowOfflineCapture": true
                }
                """),

            new FormTemplateDefinition(
                "OPTOMETRY",
                "OPTOMETRY_V1",
                "Optometría",
                "1.0.0",
                """
                {
                  "type": "object",
                  "title": "Optometría",
                  "required": ["visualComplaint", "recommendations"],
                  "properties": {
                    "visualComplaint": { "type": "string", "title": "Motivo visual" },
                    "rightEye": { "type": ["string", "null"], "title": "Ojo derecho" },
                    "leftEye": { "type": ["string", "null"], "title": "Ojo izquierdo" },
                    "lensSupportProvided": { "type": "boolean", "title": "Se entregó apoyo óptico" },
                    "recommendations": { "type": "string", "title": "Recomendaciones" },
                    "requiresReferral": { "type": "boolean", "title": "Requiere referencia" }
                  }
                }
                """,
                """
                {
                  "sections": [
                    { "title": "Valoración visual", "fields": ["visualComplaint", "rightEye", "leftEye"] },
                    { "title": "Cierre", "fields": ["lensSupportProvided", "recommendations", "requiresReferral"] }
                  ]
                }
                """,
                """
                {
                  "requiresSensitiveHandling": true,
                  "allowOfflineCapture": true
                }
                """),

            new FormTemplateDefinition(
                "NUTRITION",
                "NUTRITION_V1",
                "Nutrición",
                "1.0.0",
                """
                {
                  "type": "object",
                  "title": "Nutrición",
                  "required": ["nutritionReason", "recommendations"],
                  "properties": {
                    "nutritionReason": { "type": "string", "title": "Motivo de orientación" },
                    "weightKg": { "type": ["number", "null"], "title": "Peso kg" },
                    "heightCm": { "type": ["number", "null"], "title": "Estatura cm" },
                    "dietNotes": { "type": ["string", "null"], "title": "Notas alimentarias" },
                    "recommendations": { "type": "string", "title": "Recomendaciones" },
                    "requiresFollowUp": { "type": "boolean", "title": "Requiere seguimiento" }
                  }
                }
                """,
                """
                {
                  "sections": [
                    { "title": "Valoración nutricional", "fields": ["nutritionReason", "weightKg", "heightCm", "dietNotes"] },
                    { "title": "Cierre", "fields": ["recommendations", "requiresFollowUp"] }
                  ]
                }
                """,
                """
                {
                  "requiresSensitiveHandling": true,
                  "allowOfflineCapture": true
                }
                """),

            new FormTemplateDefinition(
                "PSYCHOLOGY",
                "PSYCHOLOGY_V1",
                "Psicología",
                "1.0.0",
                """
                {
                  "type": "object",
                  "title": "Psicología",
                  "required": ["reason", "orientationProvided"],
                  "properties": {
                    "reason": { "type": "string", "title": "Motivo de orientación" },
                    "riskObserved": { "type": "boolean", "title": "Se observó riesgo" },
                    "orientationProvided": { "type": "string", "title": "Orientación brindada" },
                    "requiresFollowUp": { "type": "boolean", "title": "Requiere seguimiento" },
                    "requiresReferral": { "type": "boolean", "title": "Requiere canalización" }
                  }
                }
                """,
                """
                {
                  "sections": [
                    { "title": "Atención psicológica", "fields": ["reason", "riskObserved", "orientationProvided"] },
                    { "title": "Cierre", "fields": ["requiresFollowUp", "requiresReferral"] }
                  ]
                }
                """,
                """
                {
                  "requiresSensitiveHandling": true,
                  "requiresExtraPrivacy": true,
                  "allowOfflineCapture": true
                }
                """),

            new FormTemplateDefinition(
                "MEDICATION_DELIVERY",
                "MEDICATION_DELIVERY_V1",
                "Entrega de medicamentos",
                "1.0.0",
                """
                {
                  "type": "object",
                  "title": "Entrega de medicamentos",
                  "required": ["medicationName", "quantity", "instructions"],
                  "properties": {
                    "medicationName": { "type": "string", "title": "Medicamento" },
                    "quantity": { "type": "string", "title": "Cantidad entregada" },
                    "lotNumber": { "type": ["string", "null"], "title": "Lote" },
                    "expirationDate": { "type": ["string", "null"], "format": "date", "title": "Caducidad" },
                    "instructions": { "type": "string", "title": "Indicaciones" }
                  }
                }
                """,
                """
                {
                  "sections": [
                    { "title": "Medicamento", "fields": ["medicationName", "quantity", "lotNumber", "expirationDate"] },
                    { "title": "Indicaciones", "fields": ["instructions"] }
                  ]
                }
                """,
                """
                {
                  "requiresSensitiveHandling": true,
                  "allowOfflineCapture": true
                }
                """),

            new FormTemplateDefinition(
                "MEDICAL_REFERRAL",
                "MEDICAL_REFERRAL_V1",
                "Referencia médica",
                "1.0.0",
                """
                {
                  "type": "object",
                  "title": "Referencia médica",
                  "required": ["reason", "referredTo", "priority"],
                  "properties": {
                    "reason": { "type": "string", "title": "Motivo de referencia" },
                    "referredTo": { "type": "string", "title": "Institución o servicio destino" },
                    "priority": { "type": "string", "title": "Prioridad", "enum": ["normal", "preferente", "urgente"] },
                    "notes": { "type": ["string", "null"], "title": "Notas" }
                  }
                }
                """,
                """
                {
                  "sections": [
                    { "title": "Referencia", "fields": ["reason", "referredTo", "priority", "notes"] }
                  ]
                }
                """,
                """
                {
                  "requiresSensitiveHandling": true,
                  "allowOfflineCapture": true
                }
                """)
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

        property.SetValue(instance, value);
    }

    private sealed record FormTemplateDefinition(
        string ServiceCode,
        string FormCode,
        string Name,
        string Version,
        string SchemaJson,
        string? UiSchemaJson,
        string? ValidationRulesJson);
}
