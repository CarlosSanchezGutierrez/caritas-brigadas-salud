using System.Reflection;
using Caritas.Brigadas.Application.Reports;
using Caritas.Brigadas.Contracts.Reports;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Reports;

public sealed class ReportReadRepository : IReportReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public ReportReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrganizationReportSummaryDto> GetOrganizationSummaryAsync(
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

        var roleIds = await _dbContext.Roles
            .AsNoTracking()
            .Where(role =>
                role.OrganizationId == organizationId &&
                !role.IsDeleted)
            .Select(role => role.Id)
            .ToListAsync(cancellationToken);

        var brigadeIds = await _dbContext.Brigades
            .AsNoTracking()
            .Where(brigade =>
                brigade.OrganizationId == organizationId &&
                !brigade.IsDeleted)
            .Select(brigade => brigade.Id)
            .ToListAsync(cancellationToken);

        var usersCount = await _dbContext.Users
            .AsNoTracking()
            .CountAsync(
                user =>
                    user.OrganizationId == organizationId &&
                    !user.IsDeleted,
                cancellationToken);

        var rolesCount = roleIds.Count;

        var permissionsCount = await _dbContext.Permissions
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var rolePermissionsCount = await _dbContext.RolePermissions
            .AsNoTracking()
            .CountAsync(
                rolePermission => roleIds.Contains(rolePermission.RoleId),
                cancellationToken);

        var servicesCount = await _dbContext.Services
            .AsNoTracking()
            .CountAsync(
                service =>
                    service.OrganizationId == organizationId &&
                    !service.IsDeleted,
                cancellationToken);

        var communitiesCount = await _dbContext.Communities
            .AsNoTracking()
            .CountAsync(
                community =>
                    community.OrganizationId == organizationId &&
                    !community.IsDeleted,
                cancellationToken);

        var mobileUnitsCount = await _dbContext.MobileUnits
            .AsNoTracking()
            .CountAsync(
                unit =>
                    unit.OrganizationId == organizationId &&
                    !unit.IsDeleted,
                cancellationToken);

        var brigadesCount = brigadeIds.Count;

        var brigadeServiceAssignmentsCount = await _dbContext.BrigadeServices
            .AsNoTracking()
            .CountAsync(
                assignment =>
                    brigadeIds.Contains(assignment.BrigadeId) &&
                    !assignment.IsDeleted,
                cancellationToken);

        var patientsCount = await _dbContext.Patients
            .AsNoTracking()
            .CountAsync(
                patient =>
                    patient.OrganizationId == organizationId &&
                    !patient.IsDeleted,
                cancellationToken);

        var patientVisitsCount = await _dbContext.PatientVisits
            .AsNoTracking()
            .CountAsync(
                visit =>
                    visit.OrganizationId == organizationId &&
                    !visit.IsDeleted,
                cancellationToken);

        var serviceEncountersCount = await _dbContext.ServiceEncounters
            .AsNoTracking()
            .CountAsync(
                encounter =>
                    encounter.OrganizationId == organizationId &&
                    !encounter.IsDeleted,
                cancellationToken);

        var formTemplates = await _dbContext.FormTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var formTemplatesCount = formTemplates.Count(template =>
            GetGuidProperty(template, "OrganizationId") == organizationId &&
            !GetBoolProperty(template, "IsDeleted"));

        var formResponses = await _dbContext.FormResponses
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var formResponsesCount = formResponses.Count(response =>
            GetGuidProperty(response, "OrganizationId") == organizationId &&
            !GetBoolProperty(response, "IsDeleted"));

        var consentDocumentsCount = await _dbContext.Set<ConsentDocument>()
            .AsNoTracking()
            .CountAsync(
                document =>
                    document.OrganizationId == organizationId &&
                    !document.IsDeleted,
                cancellationToken);

        return new OrganizationReportSummaryDto
        {
            OrganizationId = organizationId,
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            UsersCount = usersCount,
            RolesCount = rolesCount,
            PermissionsCount = permissionsCount,
            RolePermissionsCount = rolePermissionsCount,
            ServicesCount = servicesCount,
            CommunitiesCount = communitiesCount,
            MobileUnitsCount = mobileUnitsCount,
            BrigadesCount = brigadesCount,
            BrigadeServiceAssignmentsCount = brigadeServiceAssignmentsCount,
            PatientsCount = patientsCount,
            PatientVisitsCount = patientVisitsCount,
            ServiceEncountersCount = serviceEncountersCount,
            FormTemplatesCount = formTemplatesCount,
            FormResponsesCount = formResponsesCount,
            ConsentDocumentsCount = consentDocumentsCount,
            ClinicalRecordsCount = serviceEncountersCount + formResponsesCount + consentDocumentsCount
        };
    }

    private static Guid GetGuidProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);

        return value is Guid guid
            ? guid
            : Guid.Empty;
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
