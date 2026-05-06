using Caritas.Brigadas.Api.Audit;
using Caritas.Brigadas.Application.Audit;

namespace Caritas.Brigadas.Api.Extensions;

public static class AuditLoggingServiceExtensions
{
    public static IServiceCollection AddAuditLogging(
        this IServiceCollection services)
    {
        services.AddScoped<IAuditLogger, HttpAuditLogger>();

        return services;
    }
}
