using Caritas.Brigadas.Api.Audit;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Extensions;

public static class OperationalWriteAuditServiceExtensions
{
    public static IServiceCollection AddOperationalWriteAudit(
        this IServiceCollection services)
    {
        services.AddScoped<OperationalWriteAuditActionFilter>();

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.AddService<OperationalWriteAuditActionFilter>();
        });

        return services;
    }
}
