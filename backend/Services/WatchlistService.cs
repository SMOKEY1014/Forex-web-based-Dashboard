using Application.Interfaces;
using Domain.Entities;

namespace Services;

public sealed class WatchlistService(IWatchlistRepository watchlistRepository)
{
    private static readonly string[] SupportedMarkets = ["US100", "US30", "GER40", "XAUUSD"];

    public async Task<IReadOnlyList<Watchlist>> GetOrSeedWatchlistsAsync(CancellationToken cancellationToken)
    {
        var existing = await watchlistRepository.GetAllAsync(cancellationToken);
        if (existing.Count > 0)
        {
            return existing;
        }

        foreach (var market in SupportedMarkets)
        {
            await watchlistRepository.UpsertAsync(new Watchlist
            {
                Name = market,
                Market = market,
                Notes = $"System seeded watchlist for {market}"
            }, cancellationToken);
        }

        return await watchlistRepository.GetAllAsync(cancellationToken);
    }
}
