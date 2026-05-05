namespace Caritas.Brigadas.Application.Security;

public static class CurrentUserClaimTypes
{
    public const string UserId = "user_id";
    public const string OrganizationId = "organization_id";
    public const string RoleCode = "role_code";
    public const string PermissionCode = "permission_code";

    public const string LegacyUserId = "sub";
    public const string LegacyRole = "role";
}
