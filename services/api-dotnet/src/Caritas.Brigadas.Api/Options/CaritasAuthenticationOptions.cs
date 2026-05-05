namespace Caritas.Brigadas.Api.Options;

public sealed class CaritasAuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Mode { get; init; } = CaritasAuthenticationModes.Development;

    public string? Authority { get; init; }

    public string? Audience { get; init; }

    public bool RequireHttpsMetadata { get; init; } = true;

    public string? ValidIssuer { get; init; }

    public string[] ValidAudiences { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> ValidateForEnvironment(
        string environmentName)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Mode))
        {
            errors.Add("Authentication mode is required.");
            return errors;
        }

        if (!CaritasAuthenticationModes.All.Contains(
                Mode.Trim(),
                StringComparer.OrdinalIgnoreCase))
        {
            errors.Add($"Authentication mode '{Mode}' is not supported.");
            return errors;
        }

        var isDevelopment = string.Equals(
            environmentName,
            "Development",
            StringComparison.OrdinalIgnoreCase);

        if (!isDevelopment &&
            string.Equals(
                Mode,
                CaritasAuthenticationModes.Development,
                StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("Development authentication mode is only allowed in Development environment.");
        }

        if (!isDevelopment &&
            string.Equals(
                Mode,
                CaritasAuthenticationModes.Disabled,
                StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("Disabled authentication mode is not allowed outside Development environment.");
        }

        if (string.Equals(
                Mode,
                CaritasAuthenticationModes.JwtBearer,
                StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(Authority))
            {
                errors.Add("JWT Bearer authentication requires Authentication:Authority.");
            }

            if (string.IsNullOrWhiteSpace(Audience) &&
                (ValidAudiences is null || ValidAudiences.Length == 0))
            {
                errors.Add("JWT Bearer authentication requires Authentication:Audience or Authentication:ValidAudiences.");
            }
        }

        return errors;
    }
}
