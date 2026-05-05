using Caritas.Brigadas.Api.Options;

namespace Caritas.Brigadas.Api.Extensions;

public static class ConfiguredAuthenticationServiceExtensions
{
    public static IServiceCollection AddConfiguredAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var options = configuration
            .GetSection(CaritasAuthenticationOptions.SectionName)
            .Get<CaritasAuthenticationOptions>() ?? new CaritasAuthenticationOptions();

        var validationErrors = options.ValidateForEnvironment(environment.EnvironmentName);

        if (validationErrors.Count > 0)
        {
            throw new InvalidOperationException(
                "Authentication configuration is invalid: " +
                string.Join(" | ", validationErrors));
        }

        if (string.Equals(
                options.Mode,
                CaritasAuthenticationModes.Development,
                StringComparison.OrdinalIgnoreCase))
        {
            services.AddDevelopmentAuthentication(environment);
            return services;
        }

        if (string.Equals(
                options.Mode,
                CaritasAuthenticationModes.Disabled,
                StringComparison.OrdinalIgnoreCase))
        {
            if (!environment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "Disabled authentication mode is only allowed in Development environment.");
            }

            services.AddAuthorization();
            return services;
        }

        if (string.Equals(
                options.Mode,
                CaritasAuthenticationModes.JwtBearer,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                "JWT Bearer authentication mode is configured but the JWT handler has not been implemented yet.");
        }

        throw new InvalidOperationException(
            $"Unsupported authentication mode '{options.Mode}'.");
    }
}
