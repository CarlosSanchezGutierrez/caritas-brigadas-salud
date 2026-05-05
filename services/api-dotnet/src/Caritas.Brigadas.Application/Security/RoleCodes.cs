namespace Caritas.Brigadas.Application.Security;

public static class RoleCodes
{
    public const string SuperAdmin = "SUPER_ADMIN";
    public const string OrganizationAdmin = "ORGANIZATION_ADMIN";
    public const string Coordinator = "COORDINATOR";
    public const string HealthProvider = "HEALTH_PROVIDER";
    public const string Reception = "RECEPTION";
    public const string Viewer = "VIEWER";
    public const string Auditor = "AUDITOR";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        SuperAdmin,
        OrganizationAdmin,
        Coordinator,
        HealthProvider,
        Reception,
        Viewer,
        Auditor
    };
}
