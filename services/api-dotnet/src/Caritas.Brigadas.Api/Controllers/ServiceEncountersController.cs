using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.ServiceEncounters;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.ServiceEncounters;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para atenciones por servicio dentro de una visita.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class ServiceEncountersController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceEncountersController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista atenciones de servicio de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/service-encounters")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ServiceEncounterSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IServiceEncounterReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var encounters = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ServiceEncounterSummaryDto>>.Ok(
            encounters,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una atención de servicio por identificador.
    /// </summary>
    [HttpGet("api/v1/service-encounters/{encounterId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServiceEncounterSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(
        Guid encounterId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IServiceEncounterReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var encounter = await repository.GetByIdAsync(
            encounterId,
            cancellationToken);

        if (encounter is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Service encounter was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<ServiceEncounterSummaryDto>.Ok(
            encounter,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea una atención por servicio dentro de una visita.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/service-encounters")]
    [ProducesResponseType(typeof(ApiResponse<ServiceEncounterSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateServiceEncounterRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IServiceEncounterWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var encounter = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var response = ApiResponse<ServiceEncounterSummaryDto>.Ok(
                encounter,
                HttpContext.GetCorrelationId(),
                "Service encounter created successfully.");

            return Created($"/api/v1/service-encounters/{encounter.Id}", response);
        }
        catch (KeyNotFoundException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                exception.Message,
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }
        catch (InvalidOperationException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.Conflict,
                exception.Message,
                HttpContext.GetCorrelationId());

            return Conflict(error);
        }
        catch (DomainException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.ValidationError,
                exception.Message,
                HttpContext.GetCorrelationId());

            return BadRequest(error);
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
