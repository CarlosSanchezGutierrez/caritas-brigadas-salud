using Caritas.Brigadas.Api.Options;
using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

            services.AddDisabledAuthentication(environment);
            return services;
        }

        if (string.Equals(
                options.Mode,
                CaritasAuthenticationModes.JwtBearer,
                StringComparison.OrdinalIgnoreCase))
        {
            services
                .AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Authority = options.Authority;
                    jwtOptions.Audience = options.Audience;
                    jwtOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;

                    jwtOptions.MapInboundClaims = false;

                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromMinutes(2),
                        NameClaimType = "name",
                        RoleClaimType = CurrentUserClaimTypes.RoleCode
                    };

                    if (!string.IsNullOrWhiteSpace(options.ValidIssuer))
                    {
                        jwtOptions.TokenValidationParameters.ValidIssuer = options.ValidIssuer;
                    }

                    var validAudiences = new List<string>();

                    if (!string.IsNullOrWhiteSpace(options.Audience))
                    {
                        validAudiences.Add(options.Audience);
                    }

                    if (options.ValidAudiences is { Length: > 0 })
                    {
                        validAudiences.AddRange(
                            options.ValidAudiences
                                .Where(audience => !string.IsNullOrWhiteSpace(audience))
                                .Select(audience => audience.Trim()));
                    }

                    jwtOptions.TokenValidationParameters.ValidAudiences = validAudiences
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();
                });

            services.AddAuthorization();

            return services;
        }

        throw new InvalidOperationException(
            $"Unsupported authentication mode '{options.Mode}'.");
    }
}
