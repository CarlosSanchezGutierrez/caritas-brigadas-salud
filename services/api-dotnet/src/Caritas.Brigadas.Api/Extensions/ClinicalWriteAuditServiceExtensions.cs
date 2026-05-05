using Caritas.Brigadas.Api.Audit;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Extensions;

public static class ClinicalWriteAuditServiceExtensions
{
    public static IServiceCollection AddClinicalWriteAudit(
        this IServiceCollection services)
    {
        services.AddScoped<ClinicalWriteAuditActionFilter>();

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.AddService<ClinicalWriteAuditActionFilter>();
        });

        return services;
    }
}
