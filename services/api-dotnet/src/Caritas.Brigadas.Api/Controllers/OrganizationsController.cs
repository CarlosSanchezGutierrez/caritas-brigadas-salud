using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Organizations;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para organizaciones institucionales.
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
    [Authorize(Policy = PermissionCodes.OrganizationsRead)]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IOrganizationReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        if (!IsSuperAdmin(User))
        {
            var currentOrganizationIdClaim = User.FindFirst(
                Caritas.Brigadas.Application.Security.CurrentUserClaimTypes.OrganizationId)?.Value;

            if (!Guid.TryParse(currentOrganizationIdClaim, out var currentOrganizationId))
            {
                return Forbid();
            }

            var organization = await repository.GetByIdAsync(
                currentOrganizationId,
                cancellationToken);

            var scopedOrganizations = organization is null
                ? Array.Empty<OrganizationSummaryDto>()
                : new[] { organization };

            return Ok(ApiResponse<IReadOnlyCollection<OrganizationSummaryDto>>.Ok(
                scopedOrganizations,
                HttpContext.GetCorrelationId()));
        }

        var organizations = await repository.ListAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<OrganizationSummaryDto>>.Ok(
            organizations,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una organizaciÃƒÂ³n por identificador.
    /// </summary>
    [HttpGet("{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.OrganizationsRead)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IOrganizationReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
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

    /// <summary>
    /// Crea una organizaciÃƒÂ³n institucional.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrganizationSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.OrganizationsWrite)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsSuperAdmin(User))
        {
            return Forbid();
        }

        var repository = _serviceProvider.GetService<IOrganizationWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var organization = await repository.CreateAsync(
                request,
                cancellationToken);

            var response = ApiResponse<OrganizationSummaryDto>.Ok(
                organization,
                HttpContext.GetCorrelationId(),
                "Organization created successfully.");

            return Created($"/api/v1/organizations/{organization.Id}", response);
        }
        catch (InvalidOperationException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.Conflict,
                exception.Message,
                HttpContext.GetCorrelationId());

            return Conflict(error);
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

    private static bool IsSuperAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole("SUPER_ADMIN") ||
            user.Claims.Any(claim =>
                IsRoleClaimType(claim.Type) &&
                string.Equals(claim.Value, "SUPER_ADMIN", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsRoleClaimType(string claimType)
    {
        return string.Equals(claimType, "role_code", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(claimType, "role", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(claimType, "roles", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(claimType, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase);
    }
}




