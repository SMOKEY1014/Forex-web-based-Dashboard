using Application.Interfaces;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public sealed class MongoWatchlistVersionRepository(IMongoDatabase database) : IWatchlistVersionRepository
{
    private readonly IMongoCollection<WatchlistVersion> _collection = database.GetCollection<WatchlistVersion>("watchlistVersions");
    private int _indexInitialized;

    public async Task<int> GetLatestVersionAsync(string market, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        var latest = await _collection
            .Find(x => x.Market == market)
            .SortByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);

        return latest?.Version ?? 0;
    }

    public async Task SaveVersionAsync(WatchlistVersion watchlistVersion, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        await _collection.InsertOneAsync(watchlistVersion, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<WatchlistVersion>> GetVersionsAsync(string market, int limit, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        return await _collection
            .Find(x => x.Market == market)
            .SortByDescending(x => x.Version)
            .Limit(Math.Clamp(limit, 1, 100))
            .ToListAsync(cancellationToken);
    }

    public async Task<WatchlistVersion?> GetByVersionAsync(string market, int version, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        return await _collection.Find(x => x.Market == market && x.Version == version).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task EnsureIndexesAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.Exchange(ref _indexInitialized, 1) == 1)
        {
            return;
        }

        var marketVersionIndex = new CreateIndexModel<WatchlistVersion>(
            Builders<WatchlistVersion>.IndexKeys
                .Ascending(x => x.Market)
                .Descending(x => x.Version),
            new CreateIndexOptions { Unique = true });

        await _collection.Indexes.CreateOneAsync(marketVersionIndex, cancellationToken: cancellationToken);
    }
}
