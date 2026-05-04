using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Organizations;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints de lectura para organizaciones institucionales.
/// </summary>
[ApiController]
[Route("api/v1/organizations")]
[Produces("application/json")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public OrganizationsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista las organizaciones disponibles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<OrganizationSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IOrganizationReadRepository>();

        if (repository is null)
        {
            var error = ApiErrorResponse.Create(
                "database_not_configured",
                "Database access is not configured for this environment.",
                HttpContext.GetCorrelationId());

            return StatusCode(StatusCodes.Status503ServiceUnavailable, error);
        }

        var organizations = await repository.ListAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<OrganizationSummaryDto>>.Ok(
            organizations,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una organización por identificador.
    /// </summary>
    [HttpGet("{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IOrganizationReadRepository>();

        if (repository is null)
        {
            var error = ApiErrorResponse.Create(
                "database_not_configured",
                "Database access is not configured for this environment.",
                HttpContext.GetCorrelationId());

            return StatusCode(StatusCodes.Status503ServiceUnavailable, error);
        }

        var organization = await repository.GetByIdAsync(
            organizationId,
            cancellationToken);

        if (organization is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Organization was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<OrganizationSummaryDto>.Ok(
            organization,
            HttpContext.GetCorrelationId()));
    }
}
