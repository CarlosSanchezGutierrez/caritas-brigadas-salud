namespace Caritas.Brigadas.Application.Audit;

public static class AuditActionCodes
{
    public const string OrganizationCreate = "organizations.create";
    public const string UserCreate = "users.create";
    public const string RoleAssign = "roles.assign";

    public const string ServiceSeed = "services.seed";
    public const string FormTemplateSeed = "form-templates.seed";

    public const string CommunityCreate = "communities.create";
    public const string MobileUnitCreate = "mobile-units.create";
    public const string BrigadeCreate = "brigades.create";
    public const string BrigadeServiceAssign = "brigade-services.assign";

    public const string PatientCreate = "patients.create";
    public const string PatientVisitCreate = "patient-visits.create";
    public const string ServiceEncounterCreate = "service-encounters.create";
    public const string FormResponseCreate = "form-responses.create";
    public const string ConsentDocumentCreate = "consent-documents.create";

    public const string ReportSummaryRead = "reports.summary.read";
    public const string ReportSummaryExport = "reports.summary.export";

    public const string SyncBatchCreate = "sync-batches.create";
    public const string AuditLogRead = "audit-logs.read";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        OrganizationCreate,
        UserCreate,
        RoleAssign,
        ServiceSeed,
        FormTemplateSeed,
        CommunityCreate,
        MobileUnitCreate,
        BrigadeCreate,
        BrigadeServiceAssign,
        PatientCreate,
        PatientVisitCreate,
        ServiceEncounterCreate,
        FormResponseCreate,
        ConsentDocumentCreate,
        ReportSummaryRead,
        ReportSummaryExport,
        SyncBatchCreate,
        AuditLogRead
    };
}
