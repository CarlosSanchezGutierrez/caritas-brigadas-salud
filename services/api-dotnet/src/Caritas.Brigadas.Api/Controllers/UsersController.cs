using Microsoft.AspNetCore.Authorization;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Users;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para usuarios institucionales.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class UsersController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public UsersController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista usuarios de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/users")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.UsersRead)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IUserReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var users = await repository.ListByOrganizationAsync(
            organizationId,
            pagination,
            cancellationToken);

        return Ok(ApiResponse<PaginatedResponse<UserSummaryDto>>.Ok(
            users,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene un usuario por identificador.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/users/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.UsersRead)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IUserReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var user = await repository.GetByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "User was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        if (user.OrganizationId != organizationId)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "User was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<UserSummaryDto>.Ok(
            user,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea un usuario dentro de una organización.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/users")]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = PermissionCodes.UsersWrite)]
    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IUserWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var user = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var response = ApiResponse<UserSummaryDto>.Ok(
                user,
                HttpContext.GetCorrelationId(),
                "User created successfully.");

            return Created($"/api/v1/organizations/{organizationId}/users/{user.Id}", response);
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
}



