namespace Caritas.Brigadas.Application.Security;

public static class RoleCodes
{
    public const string SuperAdmin = "SUPER_ADMIN";
    public const string Admin = "ADMIN";
    public const string BrigadeCoordinator = "BRIGADE_COORDINATOR";
    public const string HealthProvider = "HEALTH_PROVIDER";
    public const string ServiceStudent = "SERVICE_STUDENT";
    public const string Auditor = "AUDITOR";
    public const string DataAnalyst = "DATA_ANALYST";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        SuperAdmin,
        Admin,
        BrigadeCoordinator,
        HealthProvider,
        ServiceStudent,
        Auditor,
        DataAnalyst
    };
}