using Caritas.Brigadas.Application.Brigades;
using Caritas.Brigadas.Application.Communities;
using Caritas.Brigadas.Application.ConsentDocuments;
using Caritas.Brigadas.Application.FormResponses;
using Caritas.Brigadas.Application.FormTemplates;
using Caritas.Brigadas.Application.MobileUnits;
using Caritas.Brigadas.Application.Organizations;
using Caritas.Brigadas.Application.PatientVisits;
using Caritas.Brigadas.Application.Patients;
using Caritas.Brigadas.Application.Reports;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Application.ServiceEncounters;
using Caritas.Brigadas.Application.Services;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Application.Users;
using Caritas.Brigadas.Infrastructure.Brigades;
using Caritas.Brigadas.Infrastructure.Communities;
using Caritas.Brigadas.Infrastructure.ConsentDocuments;
using Caritas.Brigadas.Infrastructure.FormResponses;
using Caritas.Brigadas.Infrastructure.FormTemplates;
using Caritas.Brigadas.Infrastructure.MobileUnits;
using Caritas.Brigadas.Infrastructure.Organizations;
using Caritas.Brigadas.Infrastructure.PatientVisits;
using Caritas.Brigadas.Infrastructure.Patients;
using Caritas.Brigadas.Infrastructure.Persistence;
using Caritas.Brigadas.Infrastructure.Reports;
using Caritas.Brigadas.Infrastructure.Security;
using Caritas.Brigadas.Infrastructure.ServiceEncounters;
using Caritas.Brigadas.Infrastructure.Services;
using Caritas.Brigadas.Infrastructure.Sync;
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

            services.AddScoped<IBrigadeReadRepository, BrigadeReadRepository>();
            services.AddScoped<IBrigadeWriteRepository, BrigadeWriteRepository>();
            services.AddScoped<IBrigadeServiceReadRepository, BrigadeServiceReadRepository>();
            services.AddScoped<IBrigadeServiceAssignmentRepository, BrigadeServiceAssignmentRepository>();

            services.AddScoped<IPatientReadRepository, PatientReadRepository>();
            services.AddScoped<IPatientWriteRepository, PatientWriteRepository>();

            services.AddScoped<IPatientVisitReadRepository, PatientVisitReadRepository>();
            services.AddScoped<IPatientVisitWriteRepository, PatientVisitWriteRepository>();

            services.AddScoped<IServiceEncounterReadRepository, ServiceEncounterReadRepository>();
            services.AddScoped<IServiceEncounterWriteRepository, ServiceEncounterWriteRepository>();

            services.AddScoped<IFormTemplateReadRepository, FormTemplateReadRepository>();
            services.AddScoped<IFormTemplateSeedRepository, FormTemplateSeedRepository>();

            services.AddScoped<IFormResponseReadRepository, FormResponseReadRepository>();
            services.AddScoped<IFormResponseWriteRepository, FormResponseWriteRepository>();

            services.AddScoped<IConsentDocumentReadRepository, ConsentDocumentReadRepository>();
            services.AddScoped<IConsentDocumentWriteRepository, ConsentDocumentWriteRepository>();

            services.AddScoped<IReportReadRepository, ReportReadRepository>();

            services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();
            services.AddScoped<ISyncBatchWriteRepository, SyncBatchWriteRepository>();
        }

        return services;
    }
}
