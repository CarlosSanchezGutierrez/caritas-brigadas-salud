using Caritas.Brigadas.Contracts.Security;
using Microsoft.AspNetCore.Authorization;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Communities;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Communities;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para comunidades y ubicaciones operativas.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class CommunitiesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public CommunitiesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista comunidades de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/communities")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<CommunitySummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.CommunitiesRead)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ICommunityReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var communities = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<CommunitySummaryDto>>.Ok(
            communities,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una comunidad por identificador.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/communities/{communityId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CommunitySummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.CommunitiesRead)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        Guid communityId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ICommunityReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var community = await repository.GetByIdAsync(
            communityId,
            cancellationToken);

        if (community is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Community was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        if (community.OrganizationId != organizationId)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Community was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<CommunitySummaryDto>.Ok(
            community,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea una comunidad o ubicación operativa.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/communities")]
    [ProducesResponseType(typeof(ApiResponse<CommunitySummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.CommunitiesWrite)]
    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateCommunityRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ICommunityWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var community = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var response = ApiResponse<CommunitySummaryDto>.Ok(
                community,
                HttpContext.GetCorrelationId(),
                "Community created successfully.");

            return Created($"/api/v1/organizations/{organizationId}/communities/{community.Id}", response);
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



