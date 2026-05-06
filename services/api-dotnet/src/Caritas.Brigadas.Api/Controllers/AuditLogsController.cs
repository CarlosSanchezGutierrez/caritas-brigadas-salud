using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public sealed class AuditLogsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public AuditLogsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpGet("organizations/{organizationId:guid}/audit-logs")]
    [Authorize(Policy = PermissionCodes.AuditLogsRead)]
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

        var auditLogs = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<AuditLogSummaryDto>>.Ok(
            auditLogs,
            HttpContext.GetCorrelationId()));
    }

    [HttpGet("audit-logs/{auditLogId:guid}")]
    [Authorize(Policy = PermissionCodes.AuditLogsRead)]
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

        try
        {
            var auditLog = await repository.GetByIdAsync(
                auditLogId,
                cancellationToken);

            return Ok(ApiResponse<AuditLogSummaryDto>.Ok(
                auditLog,
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
