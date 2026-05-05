using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Audit;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints de consulta de auditoría.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class AuditLogsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public AuditLogsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista eventos de auditoría de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/audit-logs")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<AuditLogSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IAuditLogReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var logs = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<AuditLogSummaryDto>>.Ok(
            logs,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene un evento de auditoría por identificador.
    /// </summary>
    [HttpGet("api/v1/audit-logs/{auditLogId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuditLogSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IAuditLogReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var log = await repository.GetByIdAsync(
            auditLogId,
            cancellationToken);

        if (log is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Audit log was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<AuditLogSummaryDto>.Ok(
            log,
            HttpContext.GetCorrelationId()));
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
