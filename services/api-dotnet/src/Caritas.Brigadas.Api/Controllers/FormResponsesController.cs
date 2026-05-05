using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Application.FormResponses;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.FormResponses;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para respuestas capturadas de formularios.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class FormResponsesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public FormResponsesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista respuestas de formularios de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/form-responses")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<FormResponseSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.FormResponsesRead)]

    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IFormResponseReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var responses = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<FormResponseSummaryDto>>.Ok(
            responses,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una respuesta de formulario por identificador.
    /// </summary>
    [HttpGet("api/v1/form-responses/{formResponseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FormResponseSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(
        Guid formResponseId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IFormResponseReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var response = await repository.GetByIdAsync(
            formResponseId,
            cancellationToken);

        if (response is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Form response was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<FormResponseSummaryDto>.Ok(
            response,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea una respuesta de formulario para una atención.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/form-responses")]
    [ProducesResponseType(typeof(ApiResponse<FormResponseSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.FormResponsesWrite)]

    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateFormResponseRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IFormResponseWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var response = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var payload = ApiResponse<FormResponseSummaryDto>.Ok(
                response,
                HttpContext.GetCorrelationId(),
                "Form response created successfully.");

            return Created($"/api/v1/form-responses/{response.Id}", payload);
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




