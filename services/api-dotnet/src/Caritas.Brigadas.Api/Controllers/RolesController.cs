using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para roles y asignaciones de seguridad.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class RolesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public RolesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista roles de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<RoleSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.RolesRead)]
    public async Task<IActionResult> ListRolesAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ISecurityReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var roles = await repository.ListRolesAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<RoleSummaryDto>>.Ok(
            roles,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Lista los roles asignados a un usuario.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/users/{userId:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<UserRoleSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.RolesRead)]
    public async Task<IActionResult> ListUserRolesAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ISecurityReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        
        if (!await UserBelongsToOrganizationAsync(organizationId, userId, cancellationToken))
        {
            return NotFound();
        }
var roles = await repository.ListUserRolesAsync(
            userId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<UserRoleSummaryDto>>.Ok(
            roles,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Asigna un rol a un usuario dentro de una organización.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/users/{userId:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<UserRoleSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.RolesAssign)]
    public async Task<IActionResult> AssignRoleAsync(
        Guid organizationId,
        Guid userId,
        [FromBody] AssignUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IUserRoleAssignmentRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var assignment = await repository.AssignRoleAsync(
                organizationId,
                userId,
                request,
                cancellationToken);

            var response = ApiResponse<UserRoleSummaryDto>.Ok(
                assignment,
                HttpContext.GetCorrelationId(),
                "Role assigned successfully.");

            return Created($"/api/v1/organizations/{organizationId}/users/{userId}/roles", response);
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
    }

    private ObjectResult DatabaseNotConfigured()
    {
        var error = ApiErrorResponse.Create(
            "database_not_configured",
            "Database access is not configured for this environment.",
            HttpContext.GetCorrelationId());

        return StatusCode(StatusCodes.Status503ServiceUnavailable, error);
    }

    private async Task<bool> UserBelongsToOrganizationAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var userRepository = _serviceProvider.GetService<IUserReadRepository>();

        if (userRepository is null)
        {
            return false;
        }

        var user = await userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        return user is not null && user.OrganizationId == organizationId;
    }
}
