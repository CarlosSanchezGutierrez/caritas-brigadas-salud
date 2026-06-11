using Caritas.Brigadas.Application.Audit;

namespace Caritas.Brigadas.Api.Audit;

public static class OperationalWriteAuditActionMapper
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

        if (normalizedPath.EndsWith("/organizations", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.OrganizationCreate;
            entityName = "Organization";
            return true;
        }

        if (normalizedPath.EndsWith("/users", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.UserCreate;
            entityName = "User";
            return true;
        }

        if (normalizedPath.EndsWith("/security/seed-defaults", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.RoleAssign;
            entityName = "SecurityDefaults";
            return true;
        }

        if (normalizedPath.EndsWith("/security/user-role-assignments", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.RoleAssign;
            entityName = "UserRoleAssignment";
            return true;
        }

        if (normalizedPath.EndsWith("/services/seed-defaults", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.ServiceSeed;
            entityName = "Service";
            return true;
        }

        if (normalizedPath.EndsWith("/form-templates/seed-defaults", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.FormTemplateSeed;
            entityName = "FormTemplate";
            return true;
        }

        if (normalizedPath.EndsWith("/communities", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.CommunityCreate;
            entityName = "Community";
            return true;
        }

        if (normalizedPath.EndsWith("/mobile-units", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.MobileUnitCreate;
            entityName = "MobileUnit";
            return true;
        }

        if (normalizedPath.EndsWith("/brigades", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.BrigadeCreate;
            entityName = "Brigade";
            return true;
        }

        if (normalizedPath.Contains("/brigades/", StringComparison.OrdinalIgnoreCase) &&
            normalizedPath.EndsWith("/services", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.BrigadeServiceAssign;
            entityName = "BrigadeService";
            return true;
        }
if (normalizedPath.EndsWith("/sync-batches", StringComparison.OrdinalIgnoreCase))
        {
            action = AuditActionCodes.SyncBatchCreate;
            entityName = "SyncBatch";
            return true;
        }

        return false;
    }
}
