using Microsoft.AspNetCore.Authorization;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.MobileUnits;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.MobileUnits;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para unidades móviles o recursos operativos de brigada.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class MobileUnitsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public MobileUnitsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista unidades móviles de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/mobile-units")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<MobileUnitSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.MobileUnitsRead)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IMobileUnitReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var units = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<MobileUnitSummaryDto>>.Ok(
            units,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una unidad móvil por identificador.
    /// </summary>
    [HttpGet("api/v1/mobile-units/{mobileUnitId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MobileUnitSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.MobileUnitsRead)]
    public async Task<IActionResult> GetByIdAsync(
        Guid mobileUnitId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IMobileUnitReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var unit = await repository.GetByIdAsync(
            mobileUnitId,
            cancellationToken);

        if (unit is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Mobile unit was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<MobileUnitSummaryDto>.Ok(
            unit,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea una unidad móvil o recurso operativo.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/mobile-units")]
    [ProducesResponseType(typeof(ApiResponse<MobileUnitSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.MobileUnitsWrite)]
    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateMobileUnitRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IMobileUnitWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var unit = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var response = ApiResponse<MobileUnitSummaryDto>.Ok(
                unit,
                HttpContext.GetCorrelationId(),
                "Mobile unit created successfully.");

            return Created($"/api/v1/mobile-units/{unit.Id}", response);
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



