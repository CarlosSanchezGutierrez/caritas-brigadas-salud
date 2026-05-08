using Caritas.Brigadas.Api.Security;
using Microsoft.AspNetCore.Authentication;

namespace Caritas.Brigadas.Api.Extensions;

public static class DisabledAuthenticationServiceExtensions
{
    public static IServiceCollection AddDisabledAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "Disabled authentication mode is only allowed in Development environment.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = DisabledAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DisabledAuthenticationDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, DisabledAuthenticationHandler>(
                DisabledAuthenticationDefaults.AuthenticationScheme,
                options => { });

        services.AddAuthorization();

        return services;
    }
}