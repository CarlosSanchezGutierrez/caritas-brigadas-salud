using Microsoft.AspNetCore.Authorization;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Brigades;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Brigades;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para servicios disponibles dentro de una brigada.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/brigades/{brigadeId:guid}/services")]
[Produces("application/json")]
public sealed class BrigadeServicesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public BrigadeServicesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista los servicios asignados a una brigada.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<BrigadeServiceAssignmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.BrigadeServicesRead)]
    public async Task<IActionResult> ListAsync(
        Guid organizationId,
        Guid brigadeId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IBrigadeServiceReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        
        if (!await BrigadeBelongsToOrganizationAsync(organizationId, brigadeId, cancellationToken))
        {
            return NotFound();
        }
var services = await repository.ListByBrigadeAsync(
            brigadeId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<BrigadeServiceAssignmentDto>>.Ok(
            services,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Asigna un servicio disponible a una brigada.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BrigadeServiceAssignmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.BrigadeServicesWrite)]
    public async Task<IActionResult> AssignAsync(
        Guid organizationId,
        Guid brigadeId,
        [FromBody] AssignBrigadeServiceRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IBrigadeServiceAssignmentRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        
        if (!await BrigadeBelongsToOrganizationAsync(organizationId, brigadeId, cancellationToken))
        {
            return NotFound();
        }
try
        {
            var assignment = await repository.AssignAsync(
                brigadeId,
                request,
                cancellationToken);

            var response = ApiResponse<BrigadeServiceAssignmentDto>.Ok(
                assignment,
                HttpContext.GetCorrelationId(),
                "Service assigned to brigade successfully.");

            return Created($"/api/v1/organizations/{organizationId}/brigades/{brigadeId}/services", response);
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



