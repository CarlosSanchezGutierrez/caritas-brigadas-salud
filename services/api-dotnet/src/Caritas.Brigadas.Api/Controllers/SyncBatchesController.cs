using Caritas.Brigadas.Contracts.Security;
using Microsoft.AspNetCore.Authorization;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Sync;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para recepción y consulta de lotes de sincronización offline.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class SyncBatchesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public SyncBatchesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista lotes de sincronización de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/sync-batches")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<SyncBatchSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.SyncBatchesRead)]

    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ISyncBatchReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var batches = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<SyncBatchSummaryDto>>.Ok(
            batches,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene un lote de sincronización por identificador.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SyncBatchSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.SyncBatchesRead)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        Guid syncBatchId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ISyncBatchReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var batch = await repository.GetByIdAsync(
            syncBatchId,
            cancellationToken);

        if (batch is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Sync batch was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        if (batch.OrganizationId != organizationId)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Sync batch was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<SyncBatchSummaryDto>.Ok(
            batch,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Recibe un lote de sincronización offline.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/sync-batches")]
    [ProducesResponseType(typeof(ApiResponse<SyncBatchSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.SyncBatchesWrite)]

    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateSyncBatchRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ISyncBatchWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var batch = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var payload = ApiResponse<SyncBatchSummaryDto>.Ok(
                batch,
                HttpContext.GetCorrelationId(),
                "Sync batch received successfully.");

            return Created($"/api/v1/sync-batches/{batch.Id}", payload);
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

