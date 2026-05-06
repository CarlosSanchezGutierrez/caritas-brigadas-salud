using Caritas.Brigadas.Api.Security;
using Microsoft.AspNetCore.Authentication;

namespace Caritas.Brigadas.Api.Extensions;

public static class DevelopmentAuthenticationServiceExtensions
{
    public static IServiceCollection AddDevelopmentAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return services;
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = DevelopmentAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DevelopmentAuthenticationDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                DevelopmentAuthenticationDefaults.AuthenticationScheme,
                options => { });

        services.AddAuthorization();

        return services;
    }
}
