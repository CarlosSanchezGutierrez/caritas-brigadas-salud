using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace Caritas.Brigadas.Api.Extensions;

public static class PermissionAuthorizationServiceExtensions
{
    public static IServiceCollection AddPermissionAuthorization(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddAuthorization(options =>
        {
            foreach (var permissionCode in PermissionCodes.All)
            {
                options.AddPolicy(permissionCode, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new PermissionRequirement(permissionCode));
                });
            }
        });

        return services;
    }
}
