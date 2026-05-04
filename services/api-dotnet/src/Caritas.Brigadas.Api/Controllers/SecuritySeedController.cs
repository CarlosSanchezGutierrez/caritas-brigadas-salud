using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Security;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints temporales de inicialización de seguridad.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/security")]
[Produces("application/json")]
public sealed class SecuritySeedController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public SecuritySeedController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Inicializa roles y permisos base de una organización.
    /// </summary>
    [HttpPost("seed-defaults")]
    [ProducesResponseType(typeof(ApiResponse<SecuritySeedResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<ISecuritySeedRepository>();

        if (repository is null)
        {
            var databaseError = ApiErrorResponse.Create(
                "database_not_configured",
                "Database access is not configured for this environment.",
                HttpContext.GetCorrelationId());

            return StatusCode(StatusCodes.Status503ServiceUnavailable, databaseError);
        }

        try
        {
            var result = await repository.SeedDefaultsAsync(
                organizationId,
                cancellationToken);

            return Ok(ApiResponse<SecuritySeedResultDto>.Ok(
                result,
                HttpContext.GetCorrelationId(),
                "Default roles and permissions were seeded successfully."));
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
}
