using Domain.Entities;

namespace Application.Interfaces;

public interface IWatchlistVersionRepository
{
    Task<int> GetLatestVersionAsync(string market, CancellationToken cancellationToken);

    Task SaveVersionAsync(WatchlistVersion watchlistVersion, CancellationToken cancellationToken);

    Task<IReadOnlyList<WatchlistVersion>> GetVersionsAsync(string market, int limit, CancellationToken cancellationToken);

    Task<WatchlistVersion?> GetByVersionAsync(string market, int version, CancellationToken cancellationToken);
}
