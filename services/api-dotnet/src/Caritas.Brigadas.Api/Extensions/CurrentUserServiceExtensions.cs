using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Security;

namespace Caritas.Brigadas.Api.Extensions;

public static class CurrentUserServiceExtensions
{
    public static IServiceCollection AddCurrentUserContext(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
        services.AddScoped<IOrganizationAccessAuthorizer, OrganizationAccessAuthorizer>();

        return services;
    }
}
