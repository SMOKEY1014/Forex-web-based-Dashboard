using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Presentation.Controllers;

[ApiController]
[Route("api/watchlists")]
public sealed class WatchlistsController(AdminWatchlistService adminWatchlistService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await adminWatchlistService.GetAllAsync(cancellationToken));
    }

    [HttpPut("{market}")]
    [Authorize(Policy = "GuestOrAuthenticated")]
    public async Task<IActionResult> Upsert(string market, [FromBody] Watchlist watchlist, CancellationToken cancellationToken)
    {
        if (!string.Equals(market, watchlist.Market, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Market in route and payload must match.");
        }

        var saved = await adminWatchlistService.UpsertAsync(new Watchlist
        {
            Id = watchlist.Id,
            Name = watchlist.Name,
            Market = market.ToUpperInvariant(),
            Companies = watchlist.Companies,
            MaximumCompanies = watchlist.MaximumCompanies,
            Notes = watchlist.Notes,
            AutoUpdateEnabled = watchlist.AutoUpdateEnabled,
            AutoWeightRefreshEnabled = watchlist.AutoWeightRefreshEnabled,
            RefreshIntervalSeconds = watchlist.RefreshIntervalSeconds,
            LastUpdatedUtc = watchlist.LastUpdatedUtc,
            CreatedUtc = watchlist.CreatedUtc,
            ModifiedUtc = DateTimeOffset.UtcNow,
            Version = watchlist.Version
        }, "update", cancellationToken);

        return Ok(saved);
    }
}
