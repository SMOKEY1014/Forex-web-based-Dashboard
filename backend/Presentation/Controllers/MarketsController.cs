using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/markets")]
[AllowAnonymous]
public sealed class MarketsController(IMarketSnapshotRepository snapshotRepository) : ControllerBase
{
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest(CancellationToken cancellationToken)
    {
        var snapshots = await snapshotRepository.GetLatestAsync(cancellationToken);
        return Ok(snapshots.OrderBy(x => x.Market));
    }

    [HttpGet("{market}/history")]
    public async Task<IActionResult> GetHistory(string market, [FromQuery] int limit = 60, CancellationToken cancellationToken = default)
    {
        var snapshots = await snapshotRepository.GetHistoryAsync(market.ToUpperInvariant(), limit, cancellationToken);
        return Ok(snapshots.OrderBy(x => x.UpdatedAtUtc));
    }
}
