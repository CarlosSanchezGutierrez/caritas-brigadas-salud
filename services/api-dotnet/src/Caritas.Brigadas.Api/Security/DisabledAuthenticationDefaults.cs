namespace Caritas.Brigadas.Api.Security;

public static class DisabledAuthenticationDefaults
{
    public const string AuthenticationScheme = "Disabled";
    public static readonly Guid UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid OrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
}