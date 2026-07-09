using Domain.Entities;

namespace Application.Interfaces;

public interface IWatchlistRepository
{
    Task<IReadOnlyList<Watchlist>> GetAllAsync(CancellationToken cancellationToken);

    Task<Watchlist?> GetByMarketAsync(string market, CancellationToken cancellationToken);

    Task<Watchlist> UpsertAsync(Watchlist watchlist, CancellationToken cancellationToken);
}
