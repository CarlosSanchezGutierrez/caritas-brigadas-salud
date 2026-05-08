using Caritas.Brigadas.Contracts.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Application.Patients;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para pacientes.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class PatientsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public PatientsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista pacientes de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/patients")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PatientSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.PatientsRead)]

    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IPatientReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var patients = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<PatientSummaryDto>>.Ok(
            patients,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene un paciente por identificador.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.PatientsRead)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IPatientReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var patient = await repository.GetByIdAsync(
            patientId,
            cancellationToken);

        if (patient is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Patient was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        if (patient.OrganizationId != organizationId)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Patient was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<PatientSummaryDto>.Ok(
            patient,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea un paciente dentro de una organización.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/patients")]
    [ProducesResponseType(typeof(ApiResponse<PatientSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.PatientsWrite)]

    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IPatientWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var patient = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var response = ApiResponse<PatientSummaryDto>.Ok(
                patient,
                HttpContext.GetCorrelationId(),
                "Patient created successfully.");

            return Created($"api/v1/organizations/{organizationId}/patients/{patient.Id}", response);
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




