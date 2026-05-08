using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Security;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Security;

public sealed class SecuritySeedRepository : ISecuritySeedRepository
{
    private readonly CaritasDbContext _dbContext;

    public SecuritySeedRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SecuritySeedResultDto> SeedDefaultsAsync(
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

        var permissionsCreated = await SeedPermissionsAsync(cancellationToken);
        var rolesCreated = await SeedRolesAsync(organizationId, cancellationToken);
        var rolePermissionsCreated = await SeedRolePermissionsAsync(organizationId, cancellationToken);

        var roleCodes = await _dbContext.Roles
            .AsNoTracking()
            .Where(role =>
                role.OrganizationId == organizationId &&
                !role.IsDeleted)
            .OrderBy(role => role.Code)
            .Select(role => role.Code)
            .ToListAsync(cancellationToken);

        var permissionCodes = await _dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Code)
            .Select(permission => permission.Code)
            .ToListAsync(cancellationToken);

        return new SecuritySeedResultDto
        {
            OrganizationId = organizationId,
            RolesCreated = rolesCreated,
            PermissionsCreated = permissionsCreated,
            RolePermissionsCreated = rolePermissionsCreated,
            RoleCodes = roleCodes,
            PermissionCodes = permissionCodes
        };
    }

    private async Task<int> SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        var definitions = GetPermissionDefinitions();

        var existingCodes = await _dbContext.Permissions
            .AsNoTracking()
            .Select(permission => permission.Code)
            .ToListAsync(cancellationToken);

        var existingSet = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var created = 0;

        foreach (var definition in definitions)
        {
            if (existingSet.Contains(definition.Code))
            {
                continue;
            }

            var permission = new Permission(
                Guid.NewGuid(),
                definition.Code,
                definition.Name,
                definition.Module,
                definition.Action,
                definition.Description,
                definition.SensitivityLevel);

            _dbContext.Permissions.Add(permission);
            created++;
        }

        if (created > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return created;
    }

    private async Task<int> SeedRolesAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var definitions = GetRoleDefinitions();

        var existingCodes = await _dbContext.Roles
            .AsNoTracking()
            .Where(role => role.OrganizationId == organizationId)
            .Select(role => role.Code)
            .ToListAsync(cancellationToken);

        var existingSet = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var created = 0;

        foreach (var definition in definitions)
        {
            if (existingSet.Contains(definition.Code))
            {
                continue;
            }

            var role = new Role(
                Guid.NewGuid(),
                organizationId,
                definition.Code,
                definition.Name,
                definition.Description,
                definition.IsSystemRole);

            _dbContext.Roles.Add(role);
            created++;
        }

        if (created > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return created;
    }

    private async Task<int> SeedRolePermissionsAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var rolePermissionMap = GetRolePermissionMap();

        var roles = await _dbContext.Roles
            .Where(role => role.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

        var permissions = await _dbContext.Permissions
            .ToListAsync(cancellationToken);

        var roleIds = roles.Select(role => role.Id).ToArray();

        var existingRolePermissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rolePermission => roleIds.Contains(rolePermission.RoleId))
            .Select(rolePermission => new
            {
                rolePermission.RoleId,
                rolePermission.PermissionId
            })
            .ToListAsync(cancellationToken);

        var created = 0;

        foreach (var role in roles)
        {
            if (!rolePermissionMap.TryGetValue(role.Code, out var permissionCodes))
            {
                continue;
            }

            foreach (var permissionCode in permissionCodes)
            {
                var permission = permissions.SingleOrDefault(item =>
                    string.Equals(item.Code, permissionCode, StringComparison.OrdinalIgnoreCase));

                if (permission is null)
                {
                    continue;
                }

                var alreadyExists = existingRolePermissions.Any(existing =>
                    existing.RoleId == role.Id &&
                    existing.PermissionId == permission.Id);

                if (alreadyExists)
                {
                    continue;
                }

                _dbContext.RolePermissions.Add(new RolePermission(
                    Guid.NewGuid(),
                    role.Id,
                    permission.Id,
                    DateTimeOffset.UtcNow));

                created++;
            }
        }

        if (created > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return created;
    }

    private static IReadOnlyCollection<RoleDefinition> GetRoleDefinitions()
    {
        return new[]
        {
            new RoleDefinition(
                "SUPER_ADMIN",
                "Superadministrador institucional",
                "Control total de la organización, configuración, usuarios, permisos, auditoría y datos.",
                true),

            new RoleDefinition(
                "ADMIN",
                "Administrador institucional",
                "Administración operativa de usuarios, brigadas, servicios y reportes.",
                true),

            new RoleDefinition(
                "BRIGADE_COORDINATOR",
                "Coordinador de brigada",
                "Coordinación de brigadas, servicios disponibles, pacientes, visitas y operación en campo.",
                false),

            new RoleDefinition(
                "HEALTH_PROVIDER",
                "Prestador de servicio de salud",
                "Usuario que brinda atención en un servicio de salud: medicina, psicología, nutrición, optometría, odontología u otro.",
                false),

            new RoleDefinition(
                "SERVICE_STUDENT",
                "Estudiante prestador de servicio",
                "Estudiante o voluntario supervisado que apoya captura, atención operativa o servicios asignados.",
                false),

            new RoleDefinition(
                "AUDITOR",
                "Auditor",
                "Usuario con permisos de consulta para revisión, trazabilidad, cumplimiento y auditoría.",
                false),

            new RoleDefinition(
                "DATA_ANALYST",
                "Analista de datos",
                "Usuario enfocado en reportes, métricas, análisis y datos agregados no sensibles.",
                false)
        };
    }

    private static IReadOnlyCollection<PermissionDefinition> GetPermissionDefinitions()
    {
        return new[]
        {
            new PermissionDefinition("audit-logs.read", "Permission audit-logs.read", "audit-logs", "read", "Allows audit-logs.read.", PermissionSensitivity.Critical),
            new PermissionDefinition("brigades.read", "Permission brigades.read", "brigades", "read", "Allows brigades.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("brigades.write", "Permission brigades.write", "brigades", "write", "Allows brigades.write.", PermissionSensitivity.Restricted),
            new PermissionDefinition("brigade-services.read", "Permission brigade-services.read", "brigade-services", "read", "Allows brigade-services.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("brigade-services.write", "Permission brigade-services.write", "brigade-services", "write", "Allows brigade-services.write.", PermissionSensitivity.Restricted),
            new PermissionDefinition("communities.read", "Permission communities.read", "communities", "read", "Allows communities.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("communities.write", "Permission communities.write", "communities", "write", "Allows communities.write.", PermissionSensitivity.Restricted),
            new PermissionDefinition("consent-documents.read", "Permission consent-documents.read", "consent-documents", "read", "Allows consent-documents.read.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("consent-documents.write", "Permission consent-documents.write", "consent-documents", "write", "Allows consent-documents.write.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("form-responses.read", "Permission form-responses.read", "form-responses", "read", "Allows form-responses.read.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("form-responses.write", "Permission form-responses.write", "form-responses", "write", "Allows form-responses.write.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("form-templates.read", "Permission form-templates.read", "form-templates", "read", "Allows form-templates.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("form-templates.seed", "Permission form-templates.seed", "form-templates", "seed", "Allows form-templates.seed.", PermissionSensitivity.Restricted),
            new PermissionDefinition("mobile-units.read", "Permission mobile-units.read", "mobile-units", "read", "Allows mobile-units.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("mobile-units.write", "Permission mobile-units.write", "mobile-units", "write", "Allows mobile-units.write.", PermissionSensitivity.Restricted),
            new PermissionDefinition("organizations.read", "Permission organizations.read", "organizations", "read", "Allows organizations.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("organizations.write", "Permission organizations.write", "organizations", "write", "Allows organizations.write.", PermissionSensitivity.Restricted),
            new PermissionDefinition("patients.read", "Permission patients.read", "patients", "read", "Allows patients.read.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("patients.write", "Permission patients.write", "patients", "write", "Allows patients.write.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("patient-visits.read", "Permission patient-visits.read", "patient-visits", "read", "Allows patient-visits.read.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("patient-visits.write", "Permission patient-visits.write", "patient-visits", "write", "Allows patient-visits.write.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("reports.export", "Permission reports.export", "reports", "export", "Allows reports.export.", PermissionSensitivity.Restricted),
            new PermissionDefinition("reports.read", "Permission reports.read", "reports", "read", "Allows reports.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("roles.assign", "Permission roles.assign", "roles", "assign", "Allows roles.assign.", PermissionSensitivity.Critical),
            new PermissionDefinition("roles.read", "Permission roles.read", "roles", "read", "Allows roles.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("service-encounters.read", "Permission service-encounters.read", "service-encounters", "read", "Allows service-encounters.read.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("service-encounters.write", "Permission service-encounters.write", "service-encounters", "write", "Allows service-encounters.write.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("services.read", "Permission services.read", "services", "read", "Allows services.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("services.seed", "Permission services.seed", "services", "seed", "Allows services.seed.", PermissionSensitivity.Restricted),
            new PermissionDefinition("sync-batches.read", "Permission sync-batches.read", "sync-batches", "read", "Allows sync-batches.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("sync-batches.write", "Permission sync-batches.write", "sync-batches", "write", "Allows sync-batches.write.", PermissionSensitivity.Restricted),
            new PermissionDefinition("users.read", "Permission users.read", "users", "read", "Allows users.read.", PermissionSensitivity.Normal),
            new PermissionDefinition("users.write", "Permission users.write", "users", "write", "Allows users.write.", PermissionSensitivity.Restricted)
        };
    }
    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetRolePermissionMap()
    {
        var allPermissions = GetPermissionDefinitions()
            .Select(permission => permission.Code)
            .ToArray();

        return new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["SUPER_ADMIN"] = allPermissions,

            ["ADMIN"] = new[]
            {
                "organizations.read",
                "organizations.write",
                "users.read",
                "users.write",
                "roles.read",
                "services.read",
                "services.seed",
                "communities.read",
                "communities.write",
                "mobile-units.read",
                "mobile-units.write",
                "brigades.read",
                "brigades.write",
                "brigade-services.read",
                "brigade-services.write",
                "patients.read",
                "patients.write",
                "patient-visits.read",
                "patient-visits.write",
                "service-encounters.read",
                "service-encounters.write",
                "form-templates.read",
                "form-templates.seed",
                "form-responses.read",
                "form-responses.write",
                "consent-documents.read",
                "consent-documents.write",
                "sync-batches.read",
                "sync-batches.write",
                "reports.read",
                "reports.export",
                "audit-logs.read"
            },

            ["BRIGADE_COORDINATOR"] = new[]
            {
                "organizations.read",
                "users.read",
                "roles.read",
                "services.read",
                "communities.read",
                "communities.write",
                "mobile-units.read",
                "mobile-units.write",
                "brigades.read",
                "brigades.write",
                "brigade-services.read",
                "brigade-services.write",
                "patients.read",
                "patients.write",
                "patient-visits.read",
                "patient-visits.write",
                "service-encounters.read",
                "service-encounters.write",
                "form-templates.read",
                "form-responses.read",
                "form-responses.write",
                "consent-documents.read",
                "consent-documents.write",
                "sync-batches.read",
                "sync-batches.write",
                "reports.read"
            },

            ["HEALTH_PROVIDER"] = new[]
            {
                "organizations.read",
                "services.read",
                "brigades.read",
                "brigade-services.read",
                "patients.read",
                "patients.write",
                "patient-visits.read",
                "patient-visits.write",
                "service-encounters.read",
                "service-encounters.write",
                "form-templates.read",
                "form-responses.read",
                "form-responses.write",
                "consent-documents.read",
                "consent-documents.write",
                "sync-batches.write"
            },

            ["AUDITOR"] = new[]
            {
                "organizations.read",
                "users.read",
                "roles.read",
                "services.read",
                "communities.read",
                "mobile-units.read",
                "brigades.read",
                "brigade-services.read",
                "patients.read",
                "patient-visits.read",
                "service-encounters.read",
                "form-templates.read",
                "form-responses.read",
                "consent-documents.read",
                "sync-batches.read",
                "reports.read",
                "audit-logs.read"
            },

            ["DATA_ANALYST"] = new[]
            {
                "organizations.read",
                "communities.read",
                "mobile-units.read",
                "brigades.read",
                "brigade-services.read",
                "patients.read",
                "patient-visits.read",
                "service-encounters.read",
                "form-responses.read",
                "consent-documents.read",
                "sync-batches.read",
                "reports.read",
                "reports.export"
            }
        };
    }
    private sealed record RoleDefinition(
        string Code,
        string Name,
        string Description,
        bool IsSystemRole);

    private sealed record PermissionDefinition(
        string Code,
        string Name,
        string Module,
        string Action,
        string Description,
        string SensitivityLevel);
}
