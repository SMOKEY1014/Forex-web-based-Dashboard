using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

namespace Presentation.Controllers;

[ApiController]
[Route("api/admin/watchlists")]
[Authorize(Policy = "GuestOrAuthenticated")]
public sealed class AdminWatchlistsController(AdminWatchlistService adminWatchlistService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await adminWatchlistService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{market}")]
    public async Task<IActionResult> GetByMarket(string market, CancellationToken cancellationToken)
    {
        var watchlist = await adminWatchlistService.GetByMarketAsync(market, cancellationToken);
        return watchlist is null ? NotFound() : Ok(watchlist);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Watchlist watchlist, CancellationToken cancellationToken)
    {
        var saved = await adminWatchlistService.UpsertAsync(watchlist, "create", cancellationToken);
        return CreatedAtAction(nameof(GetByMarket), new { market = saved.Market }, saved);
    }

    [HttpPut("{market}")]
    public async Task<IActionResult> Update(string market, [FromBody] Watchlist watchlist, CancellationToken cancellationToken)
    {
        if (!string.Equals(market, watchlist.Market, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Market in route and payload must match.");
        }

        var saved = await adminWatchlistService.UpsertAsync(watchlist, "update", cancellationToken);
        return Ok(saved);
    }

    [HttpDelete("{market}")]
    public async Task<IActionResult> Delete(string market, CancellationToken cancellationToken)
    {
        var deleted = await adminWatchlistService.DeleteAsync(market, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        var payload = await adminWatchlistService.ExportAsync(cancellationToken);
        return Content(payload, "application/json");
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] JsonElement payload, CancellationToken cancellationToken)
    {
        if (payload.ValueKind != JsonValueKind.Array)
        {
            return BadRequest("Payload must be an array of watchlists.");
        }

        var parsed = payload.Deserialize<List<Watchlist>>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];

        var imported = await adminWatchlistService.ImportAsync(parsed, cancellationToken);
        return Ok(imported);
    }

    [HttpGet("{market}/versions")]
    public async Task<IActionResult> GetVersions(string market, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        return Ok(await adminWatchlistService.GetVersionsAsync(market, limit, cancellationToken));
    }

    [HttpPost("{market}/rollback/{version:int}")]
    public async Task<IActionResult> Rollback(string market, int version, CancellationToken cancellationToken)
    {
        var restored = await adminWatchlistService.RollbackAsync(market, version, cancellationToken);
        return restored is null ? NotFound() : Ok(restored);
    }
}
