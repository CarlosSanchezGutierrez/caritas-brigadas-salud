using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Contracts.Api;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints básicos para verificar el estado operativo de la API.
/// </summary>
[ApiController]
[Route("api/v1/health")]
[Produces("application/json")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Verifica que la API principal esté respondiendo.
    /// </summary>
    /// <returns>Estado básico de salud de la API.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        var payload = new
        {
            service = "caritas-brigadas-api",
            status = "healthy",
            timestampUtc = DateTimeOffset.UtcNow
        };

        return Ok(ApiResponse<object>.Ok(
            payload,
            HttpContext.GetCorrelationId()));
    }
}
