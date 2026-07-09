using Application.Interfaces;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public sealed class MongoWatchlistRepository(IMongoDatabase database) : IWatchlistRepository
{
    private readonly IMongoCollection<Watchlist> _collection = database.GetCollection<Watchlist>("watchlists");
    private int _indexInitialized;

    public async Task<IReadOnlyList<Watchlist>> GetAllAsync(CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        return await _collection.Find(FilterDefinition<Watchlist>.Empty).ToListAsync(cancellationToken);
    }

    public async Task<Watchlist?> GetByMarketAsync(string market, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        return await _collection.Find(x => x.Market == market).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Watchlist> UpsertAsync(Watchlist watchlist, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        await _collection.ReplaceOneAsync(x => x.Market == watchlist.Market, watchlist, new ReplaceOptions { IsUpsert = true }, cancellationToken);
        return watchlist;
    }

    public async Task<bool> DeleteAsync(string market, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        var result = await _collection.DeleteOneAsync(x => x.Market == market, cancellationToken);
        return result.DeletedCount > 0;
    }

    private async Task EnsureIndexesAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.Exchange(ref _indexInitialized, 1) == 1)
        {
            return;
        }

        var marketIndex = new CreateIndexModel<Watchlist>(Builders<Watchlist>.IndexKeys.Ascending(x => x.Market), new CreateIndexOptions { Unique = true });
        await _collection.Indexes.CreateOneAsync(marketIndex, cancellationToken: cancellationToken);
    }
}
