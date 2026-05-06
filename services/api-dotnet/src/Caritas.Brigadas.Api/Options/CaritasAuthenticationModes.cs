namespace Caritas.Brigadas.Api.Options;

public static class CaritasAuthenticationModes
{
    public const string Development = "Development";
    public const string JwtBearer = "JwtBearer";
    public const string Disabled = "Disabled";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Development,
        JwtBearer,
        Disabled
    };
}
