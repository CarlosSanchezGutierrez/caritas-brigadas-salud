using Caritas.Brigadas.Contracts.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Application.PatientVisits;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para visitas de pacientes a brigadas.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class PatientVisitsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public PatientVisitsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista visitas de pacientes de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/patient-visits")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PatientVisitSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.PatientVisitsRead)]

    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IPatientVisitReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var visits = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<PatientVisitSummaryDto>>.Ok(
            visits,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una visita por identificador.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/patient-visits/{visitId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientVisitSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.PatientVisitsRead)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        Guid visitId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IPatientVisitReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var visit = await repository.GetByIdAsync(
            visitId,
            cancellationToken);

        if (visit is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Patient visit was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        if (visit.OrganizationId != organizationId)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Patient visit was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<PatientVisitSummaryDto>.Ok(
            visit,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea una visita de paciente a una brigada.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/patient-visits")]
    [ProducesResponseType(typeof(ApiResponse<PatientVisitSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.PatientVisitsWrite)]

    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreatePatientVisitRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IPatientVisitWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var visit = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var response = ApiResponse<PatientVisitSummaryDto>.Ok(
                visit,
                HttpContext.GetCorrelationId(),
                "Patient visit created successfully.");

            return Created($"/api/v1/organizations/{organizationId}/patient-visits/{visit.Id}", response);
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




