namespace Caritas.Brigadas.Application.Security;

public static class PermissionCodes
{
    public const string OrganizationsRead = "organizations.read";
    public const string OrganizationsWrite = "organizations.write";

    public const string UsersRead = "users.read";
    public const string UsersWrite = "users.write";

    public const string RolesRead = "roles.read";
    public const string RolesAssign = "roles.assign";

    public const string ServicesRead = "services.read";
    public const string ServicesSeed = "services.seed";

    public const string CommunitiesRead = "communities.read";
    public const string CommunitiesWrite = "communities.write";

    public const string MobileUnitsRead = "mobile-units.read";
    public const string MobileUnitsWrite = "mobile-units.write";

    public const string BrigadesRead = "brigades.read";
    public const string BrigadesWrite = "brigades.write";

    public const string BrigadeServicesRead = "brigade-services.read";
    public const string BrigadeServicesWrite = "brigade-services.write";

    public const string PatientsRead = "patients.read";
    public const string PatientsWrite = "patients.write";

    public const string PatientVisitsRead = "patient-visits.read";
    public const string PatientVisitsWrite = "patient-visits.write";

    public const string ServiceEncountersRead = "service-encounters.read";
    public const string ServiceEncountersWrite = "service-encounters.write";

    public const string FormTemplatesRead = "form-templates.read";
    public const string FormTemplatesSeed = "form-templates.seed";

    public const string FormResponsesRead = "form-responses.read";
    public const string FormResponsesWrite = "form-responses.write";

    public const string ConsentDocumentsRead = "consent-documents.read";
    public const string ConsentDocumentsWrite = "consent-documents.write";

    public const string ReportsRead = "reports.read";
    public const string ReportsExport = "reports.export";

    public const string SyncBatchesRead = "sync-batches.read";
    public const string SyncBatchesWrite = "sync-batches.write";

    public const string AuditLogsRead = "audit-logs.read";

    public static readonly IReadOnlyCollection<string> GlobalOnly = new[]
    {
        OrganizationsWrite
    };

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        OrganizationsRead,
        OrganizationsWrite,
        UsersRead,
        UsersWrite,
        RolesRead,
        RolesAssign,
        ServicesRead,
        ServicesSeed,
        CommunitiesRead,
        CommunitiesWrite,
        MobileUnitsRead,
        MobileUnitsWrite,
        BrigadesRead,
        BrigadesWrite,
        BrigadeServicesRead,
        BrigadeServicesWrite,
        PatientsRead,
        PatientsWrite,
        PatientVisitsRead,
        PatientVisitsWrite,
        ServiceEncountersRead,
        ServiceEncountersWrite,
        FormTemplatesRead,
        FormTemplatesSeed,
        FormResponsesRead,
        FormResponsesWrite,
        ConsentDocumentsRead,
        ConsentDocumentsWrite,
        ReportsRead,
        ReportsExport,
        SyncBatchesRead,
        SyncBatchesWrite,
        AuditLogsRead
    };
}
