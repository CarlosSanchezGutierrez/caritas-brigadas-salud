using System.Reflection;
using Microsoft.OpenApi;

namespace Caritas.Brigadas.Api.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddCaritasSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Cáritas Brigadas de Salud API",
                Version = "v1",
                Description = """
                API institucional para registro, sincronización, auditoría, análisis y reportes de brigadas de salud de Cáritas de Monterrey.

                Esta API es consumida por Web/PWA, iOS y Android. Las aplicaciones cliente nunca deben conectarse directamente a SQL Server.
                """
            });

            options.CustomSchemaIds(type =>
                type.FullName?.Replace("+", ".") ?? type.Name);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    public static WebApplication UseCaritasSwagger(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "openapi/{documentName}/openapi.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";
            options.SwaggerEndpoint("/openapi/v1/openapi.json", "Cáritas Brigadas de Salud API v1");
            options.DocumentTitle = "Cáritas Brigadas de Salud API";
            options.DisplayRequestDuration();
        });

        return app;
    }
}
