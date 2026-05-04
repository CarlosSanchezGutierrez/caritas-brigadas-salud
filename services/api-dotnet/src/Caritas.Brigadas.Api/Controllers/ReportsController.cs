using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Reports;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para reportes operativos agregados.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/reports")]
[Produces("application/json")]
public sealed class ReportsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ReportsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Obtiene resumen operativo general de una organización.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationReportSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetSummaryAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IReportReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var summary = await repository.GetOrganizationSummaryAsync(
                organizationId,
                cancellationToken);

            return Ok(ApiResponse<OrganizationReportSummaryDto>.Ok(
                summary,
                HttpContext.GetCorrelationId()));
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
