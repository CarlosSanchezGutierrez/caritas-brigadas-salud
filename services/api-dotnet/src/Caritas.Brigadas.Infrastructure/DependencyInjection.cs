using Caritas.Brigadas.Application.Communities;
using Caritas.Brigadas.Application.MobileUnits;
using Caritas.Brigadas.Application.Organizations;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Application.Services;
using Caritas.Brigadas.Application.Users;
using Caritas.Brigadas.Infrastructure.Communities;
using Caritas.Brigadas.Infrastructure.MobileUnits;
using Caritas.Brigadas.Infrastructure.Organizations;
using Caritas.Brigadas.Infrastructure.Persistence;
using Caritas.Brigadas.Infrastructure.Security;
using Caritas.Brigadas.Infrastructure.Services;
using Caritas.Brigadas.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Caritas.Brigadas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var sqlServerConnectionString = configuration.GetConnectionString("SqlServer");

        if (!string.IsNullOrWhiteSpace(sqlServerConnectionString))
        {
            services.AddDbContext<CaritasDbContext>(options =>
            {
                options.UseSqlServer(
                    sqlServerConnectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(CaritasDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            });

            services.AddScoped<IOrganizationReadRepository, OrganizationReadRepository>();
            services.AddScoped<IOrganizationWriteRepository, OrganizationWriteRepository>();

            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();

            services.AddScoped<ISecuritySeedRepository, SecuritySeedRepository>();
            services.AddScoped<ISecurityReadRepository, SecurityReadRepository>();
            services.AddScoped<IUserRoleAssignmentRepository, UserRoleAssignmentRepository>();

            services.AddScoped<IServiceReadRepository, ServiceReadRepository>();
            services.AddScoped<IServiceSeedRepository, ServiceSeedRepository>();

            services.AddScoped<ICommunityReadRepository, CommunityReadRepository>();
            services.AddScoped<ICommunityWriteRepository, CommunityWriteRepository>();

            services.AddScoped<IMobileUnitReadRepository, MobileUnitReadRepository>();
            services.AddScoped<IMobileUnitWriteRepository, MobileUnitWriteRepository>();
        }

        return services;
    }
}
