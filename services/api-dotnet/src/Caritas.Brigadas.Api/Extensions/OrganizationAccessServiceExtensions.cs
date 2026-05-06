using Caritas.Brigadas.Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Extensions;

public static class OrganizationAccessServiceExtensions
{
    public static IServiceCollection AddOrganizationAccessEnforcement(
        this IServiceCollection services)
    {
        services.AddScoped<OrganizationAccessActionFilter>();

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.AddService<OrganizationAccessActionFilter>();
        });

        return services;
    }
}
