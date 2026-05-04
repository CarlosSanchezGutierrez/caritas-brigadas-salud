using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Brigades;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Brigades;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para brigadas operativas.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class BrigadesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public BrigadesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista brigadas de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/brigades")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<BrigadeSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IBrigadeReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var brigades = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<BrigadeSummaryDto>>.Ok(
            brigades,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una brigada por identificador.
    /// </summary>
    [HttpGet("api/v1/brigades/{brigadeId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BrigadeSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(
        Guid brigadeId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IBrigadeReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var brigade = await repository.GetByIdAsync(
            brigadeId,
            cancellationToken);

        if (brigade is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Brigade was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<BrigadeSummaryDto>.Ok(
            brigade,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea una brigada planificada.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/brigades")]
    [ProducesResponseType(typeof(ApiResponse<BrigadeSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateBrigadeRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IBrigadeWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var brigade = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var response = ApiResponse<BrigadeSummaryDto>.Ok(
                brigade,
                HttpContext.GetCorrelationId(),
                "Brigade created successfully.");

            return Created($"/api/v1/brigades/{brigade.Id}", response);
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
