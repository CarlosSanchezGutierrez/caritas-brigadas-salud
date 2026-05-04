using Caritas.Brigadas.Application.Services;
using Caritas.Brigadas.Contracts.Services;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Services;

public sealed class ServiceSeedRepository : IServiceSeedRepository
{
    private readonly CaritasDbContext _dbContext;

    public ServiceSeedRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceSeedResultDto> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Id == organizationId &&
                    !organization.IsDeleted,
                cancellationToken);

        if (!organizationExists)
        {
            throw new KeyNotFoundException("Organization was not found.");
        }

        var definitions = GetServiceDefinitions();

        var existingCodes = await _dbContext.Services
            .AsNoTracking()
            .Where(service => service.OrganizationId == organizationId)
            .Select(service => service.Code)
            .ToListAsync(cancellationToken);

        var existingSet = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var created = 0;

        foreach (var definition in definitions)
        {
            if (existingSet.Contains(definition.Code))
            {
                continue;
            }

            var service = new Service(
                Guid.NewGuid(),
                organizationId,
                definition.Code,
                definition.Name,
                definition.Category,
                definition.Description,
                definition.RequiresConsent,
                definition.RequiresClinicalNotes,
                definition.RequiresFollowUpOption,
                definition.RequiresReferralOption,
                definition.IsSensitive);

            _dbContext.Services.Add(service);
            created++;
        }

        if (created > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var serviceCodes = await _dbContext.Services
            .AsNoTracking()
            .Where(service =>
                service.OrganizationId == organizationId &&
                !service.IsDeleted)
            .OrderBy(service => service.Code)
            .Select(service => service.Code)
            .ToListAsync(cancellationToken);

        return new ServiceSeedResultDto
        {
            OrganizationId = organizationId,
            ServicesCreated = created,
            ServiceCodes = serviceCodes
        };
    }

    private static IReadOnlyCollection<ServiceDefinition> GetServiceDefinitions()
    {
        return new[]
        {
            new ServiceDefinition(
                ServiceCode.GeneralMedicine,
                "Medicina general",
                "Atención médica",
                "Consulta médica general durante brigada.",
                true,
                true,
                true,
                true,
                true),

            new ServiceDefinition(
                ServiceCode.Dentistry,
                "Odontología",
                "Salud bucal",
                "Atención odontológica básica, valoración y orientación.",
                true,
                true,
                true,
                true,
                true),

            new ServiceDefinition(
                ServiceCode.Optometry,
                "Optometría",
                "Salud visual",
                "Valoración visual, orientación y apoyo óptico cuando aplique.",
                true,
                true,
                true,
                true,
                true),

            new ServiceDefinition(
                ServiceCode.Nutrition,
                "Nutrición",
                "Nutrición",
                "Orientación nutricional, valoración básica y recomendaciones.",
                true,
                true,
                true,
                false,
                true),

            new ServiceDefinition(
                ServiceCode.Psychology,
                "Psicología",
                "Salud mental",
                "Orientación psicológica, escucha inicial y canalización cuando aplique.",
                true,
                true,
                true,
                true,
                true),

            new ServiceDefinition(
                ServiceCode.MedicationDelivery,
                "Entrega de medicamentos",
                "Apoyo farmacológico",
                "Registro de entrega de medicamento, lote, indicaciones y firma.",
                true,
                false,
                false,
                false,
                true),

            new ServiceDefinition(
                ServiceCode.MedicalReferral,
                "Referencia médica",
                "Canalización",
                "Emisión o registro de referencia hacia otra institución o servicio.",
                true,
                true,
                true,
                true,
                true),

            new ServiceDefinition(
                ServiceCode.Other,
                "Otro servicio",
                "General",
                "Servicio no clasificado o apoyo operativo especial.",
                false,
                false,
                false,
                false,
                false)
        };
    }

    private sealed record ServiceDefinition(
        string Code,
        string Name,
        string Category,
        string Description,
        bool RequiresConsent,
        bool RequiresClinicalNotes,
        bool RequiresFollowUpOption,
        bool RequiresReferralOption,
        bool IsSensitive);
}
