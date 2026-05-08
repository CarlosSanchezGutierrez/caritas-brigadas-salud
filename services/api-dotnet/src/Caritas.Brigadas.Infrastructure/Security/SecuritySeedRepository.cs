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
            new PermissionDefinition("organizations.read", "Leer organizaciones", "organizations", "read", "Consultar organizaciones.", PermissionSensitivity.Normal),
            new PermissionDefinition("organizations.manage", "Administrar organizaciones", "organizations", "manage", "Administrar configuración institucional.", PermissionSensitivity.Restricted),

            new PermissionDefinition("users.read", "Leer usuarios", "users", "read", "Consultar usuarios institucionales.", PermissionSensitivity.Normal),
            new PermissionDefinition("users.create", "Crear usuarios", "users", "create", "Crear usuarios institucionales.", PermissionSensitivity.Restricted),
            new PermissionDefinition("users.update", "Actualizar usuarios", "users", "update", "Actualizar usuarios institucionales.", PermissionSensitivity.Restricted),

            new PermissionDefinition("roles.read", "Leer roles", "roles", "read", "Consultar roles y permisos.", PermissionSensitivity.Normal),
            new PermissionDefinition("roles.seed", "Inicializar roles", "roles", "seed", "Inicializar roles y permisos base.", PermissionSensitivity.Critical),
            new PermissionDefinition("roles.manage", "Administrar roles", "roles", "manage", "Modificar roles y permisos.", PermissionSensitivity.Critical),
            new PermissionDefinition("roles.assign", "Asignar roles", "roles", "assign", "Asignar roles a usuarios.", PermissionSensitivity.Critical),

            new PermissionDefinition("brigades.read", "Leer brigadas", "brigades", "read", "Consultar brigadas.", PermissionSensitivity.Normal),
            new PermissionDefinition("brigades.create", "Crear brigadas", "brigades", "create", "Crear brigadas.", PermissionSensitivity.Restricted),
            new PermissionDefinition("brigades.update", "Actualizar brigadas", "brigades", "update", "Actualizar brigadas.", PermissionSensitivity.Restricted),
            new PermissionDefinition("brigades.open", "Abrir brigadas", "brigades", "open", "Iniciar operación de una brigada.", PermissionSensitivity.Restricted),
            new PermissionDefinition("brigades.close", "Cerrar brigadas", "brigades", "close", "Cerrar operación de una brigada.", PermissionSensitivity.Restricted),

            new PermissionDefinition("patients.read", "Leer pacientes", "patients", "read", "Consultar pacientes.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("patients.create", "Crear pacientes", "patients", "create", "Registrar pacientes.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("patients.update", "Actualizar pacientes", "patients", "update", "Actualizar datos de pacientes.", PermissionSensitivity.Sensitive),

            new PermissionDefinition("visits.read", "Leer visitas", "visits", "read", "Consultar visitas de pacientes.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("visits.create", "Crear visitas", "visits", "create", "Registrar visitas de pacientes.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("visits.close", "Cerrar visitas", "visits", "close", "Cerrar visitas de pacientes.", PermissionSensitivity.Sensitive),

            new PermissionDefinition("encounters.read", "Leer atenciones", "encounters", "read", "Consultar atenciones por servicio.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("encounters.create", "Crear atenciones", "encounters", "create", "Registrar atenciones por servicio.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("encounters.update", "Actualizar atenciones", "encounters", "update", "Actualizar atenciones por servicio.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("encounters.complete", "Completar atenciones", "encounters", "complete", "Completar atenciones por servicio.", PermissionSensitivity.Sensitive),

            new PermissionDefinition("documents.sign", "Firmar documentos", "documents", "sign", "Capturar firmas y consentimientos.", PermissionSensitivity.Sensitive),
            new PermissionDefinition("documents.read", "Leer documentos", "documents", "read", "Consultar documentos asociados.", PermissionSensitivity.Sensitive),

            new PermissionDefinition("sync.submit", "Enviar sincronización", "sync", "submit", "Enviar lotes offline para sincronización.", PermissionSensitivity.Restricted),
            new PermissionDefinition("sync.read", "Leer sincronización", "sync", "read", "Consultar eventos y lotes de sincronización.", PermissionSensitivity.Restricted),

            new PermissionDefinition("reports.read", "Leer reportes", "reports", "read", "Consultar reportes operativos.", PermissionSensitivity.Restricted),
            new PermissionDefinition("reports.export", "Exportar reportes", "reports", "export", "Exportar reportes institucionales.", PermissionSensitivity.Restricted),

            new PermissionDefinition("audit.read", "Leer auditoría", "audit", "read", "Consultar auditoría y trazabilidad.", PermissionSensitivity.Critical),
            new PermissionDefinition("ai.request", "Solicitar apoyo de IA", "ai", "request", "Solicitar funciones administrativas o analíticas de IA.", PermissionSensitivity.Restricted),
            new PermissionDefinition("crypto.verify", "Verificar integridad criptográfica", "crypto", "verify", "Verificar hashes y trazabilidad criptográfica.", PermissionSensitivity.Critical)
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
                "users.read",
                "users.create",
                "users.update",
                "roles.read",
                "brigades.read",
                "brigades.create",
                "brigades.update",
                "brigades.open",
                "brigades.close",
                "patients.read",
                "visits.read",
                "encounters.read",
                "documents.read",
                "sync.read",
                "reports.read",
                "reports.export"
            },

            ["BRIGADE_COORDINATOR"] = new[]
            {
                "organizations.read",
                "users.read",
                "brigades.read",
                "brigades.update",
                "brigades.open",
                "brigades.close",
                "patients.read",
                "patients.create",
                "patients.update",
                "visits.read",
                "visits.create",
                "visits.close",
                "encounters.read",
                "documents.sign",
                "documents.read",
                "sync.submit",
                "reports.read"
            },

            ["HEALTH_PROVIDER"] = new[]
            {
                "organizations.read",
                "brigades.read",
                "patients.read",
                "patients.create",
                "patients.update",
                "visits.read",
                "encounters.read",
                "encounters.create",
                "encounters.update",
                "encounters.complete",
                "documents.sign",
                "documents.read",
                "sync.submit"
            },

            ["SERVICE_STUDENT"] = new[]
            {
                "organizations.read",
                "brigades.read",
                "patients.create",
                "visits.create",
                "encounters.create",
                "documents.sign",
                "sync.submit"
            },

            ["AUDITOR"] = new[]
            {
                "organizations.read",
                "users.read",
                "roles.read",
                "brigades.read",
                "patients.read",
                "visits.read",
                "encounters.read",
                "documents.read",
                "sync.read",
                "reports.read",
                "audit.read",
                "crypto.verify"
            },

            ["DATA_ANALYST"] = new[]
            {
                "organizations.read",
                "brigades.read",
                "reports.read",
                "reports.export",
                "ai.request"
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
