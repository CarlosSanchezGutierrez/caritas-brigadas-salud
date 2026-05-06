namespace Caritas.Brigadas.Api.Security;

public static class DevelopmentAuthenticationDefaults
{
    public const string AuthenticationScheme = "Development";
    public const string UserIdHeaderName = "X-Dev-User-Id";
    public const string OrganizationIdHeaderName = "X-Dev-Organization-Id";
    public const string RolesHeaderName = "X-Dev-Roles";
    public const string PermissionsHeaderName = "X-Dev-Permissions";
    public const string NameHeaderName = "X-Dev-Name";
    public const string EmailHeaderName = "X-Dev-Email";
}
