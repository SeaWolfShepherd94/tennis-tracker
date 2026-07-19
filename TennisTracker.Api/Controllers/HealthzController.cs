using Microsoft.AspNetCore.Mvc;
using TennisTracker.Api.Interfaces;

namespace TennisTracker.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public sealed class HealthzController(IHealthzService healthzService) : ControllerBase
{
    private readonly IHealthzService _healthzService = healthzService
        ?? throw new ArgumentNullException(nameof(healthzService));

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        bool healthy = await _healthzService.IsHealthyAsync().ConfigureAwait(false);
        return healthy ? Ok(new { status = "healthy" }) : StatusCode(503, new { status = "unhealthy" });
    }
}
