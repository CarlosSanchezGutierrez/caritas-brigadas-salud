using Caritas.Brigadas.Api.Options;

namespace Caritas.Brigadas.Api.Extensions;

public static class CaritasAuthenticationOptionsServiceExtensions
{
    public static IServiceCollection AddCaritasAuthenticationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CaritasAuthenticationOptions>(
            configuration.GetSection(CaritasAuthenticationOptions.SectionName));

        return services;
    }
}
