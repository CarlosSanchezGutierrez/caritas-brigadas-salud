using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Contracts.Api;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
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
