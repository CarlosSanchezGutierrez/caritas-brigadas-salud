using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Services;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para servicios disponibles en la organización.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/services")]
[Produces("application/json")]
public sealed class ServicesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ServicesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista servicios configurados para una organización.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ServiceSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ListAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IServiceReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var services = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ServiceSummaryDto>>.Ok(
            services,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Inicializa servicios base de una organización.
    /// </summary>
    [HttpPost("seed-defaults")]
    [ProducesResponseType(typeof(ApiResponse<ServiceSeedResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IServiceSeedRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var result = await repository.SeedDefaultsAsync(
                organizationId,
                cancellationToken);

            return Ok(ApiResponse<ServiceSeedResultDto>.Ok(
                result,
                HttpContext.GetCorrelationId(),
                "Default services were seeded successfully."));
        }
        catch (KeyNotFoundException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                exception.Message,
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }
    }

    private ObjectResult DatabaseNotConfigured()
    {
        var error = ApiErrorResponse.Create(
            "database_not_configured",
            "Database access is not configured for this environment.",
            HttpContext.GetCorrelationId());

        return StatusCode(StatusCodes.Status503ServiceUnavailable, error);
    }
}
