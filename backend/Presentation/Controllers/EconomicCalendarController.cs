using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/economic-calendar")]
[AllowAnonymous]
public sealed class EconomicCalendarController(IEconomicCalendarService economicCalendarService) : ControllerBase
{
    [HttpGet("{market}")]
    public async Task<IActionResult> GetUpcoming(string market, [FromQuery] int hours = 6, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var events = await economicCalendarService.GetUpcomingAsync(
            market.ToUpperInvariant(),
            now,
            now.AddHours(Math.Clamp(hours, 1, 48)),
            cancellationToken);
        return Ok(events);
    }

    [HttpPost("ingest")]
    [Authorize(Policy = "GuestOrAuthenticated")]
    public async Task<IActionResult> Ingest(CancellationToken cancellationToken)
    {
        await economicCalendarService.IngestLatestAsync(cancellationToken);
        return Accepted();
    }
}
