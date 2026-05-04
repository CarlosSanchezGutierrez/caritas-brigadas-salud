using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "caritas-brigadas-api",
            status = "healthy",
            timestampUtc = DateTimeOffset.UtcNow
        });
    }
}
