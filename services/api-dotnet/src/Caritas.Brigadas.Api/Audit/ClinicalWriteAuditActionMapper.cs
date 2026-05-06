using Caritas.Brigadas.Application.Audit;

namespace Caritas.Brigadas.Api.Audit;

public static class ClinicalWriteAuditActionMapper
{
    public static bool TryMap(
        string method,
        string? path,
        out string action,
        out string entityName)
    {
        action = string.Empty;
        entityName = string.Empty;

        if (!string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        var normalizedPath = path.Trim().ToLowerInvariant();

        if (normalizedPath.EndsWith("/patients", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.PatientCreate;
            entityName = "Patient";
            return true;
        }

        if (normalizedPath.EndsWith("/patient-visits", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.PatientVisitCreate;
            entityName = "PatientVisit";
            return true;
        }

        if (normalizedPath.EndsWith("/service-encounters", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.ServiceEncounterCreate;
            entityName = "ServiceEncounter";
            return true;
        }

        if (normalizedPath.EndsWith("/form-responses", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.FormResponseCreate;
            entityName = "FormResponse";
            return true;
        }

        if (normalizedPath.EndsWith("/consent-documents", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.ConsentDocumentCreate;
            entityName = "ConsentDocument";
            return true;
        }

        return false;
    }
}
