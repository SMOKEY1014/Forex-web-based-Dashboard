using Application.Interfaces;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public sealed class MongoMarketSnapshotRepository(IMongoDatabase database) : IMarketSnapshotRepository
{
    private readonly IMongoCollection<MarketSnapshot> _collection = database.GetCollection<MarketSnapshot>("marketSnapshots");
    private int _indexInitialized;

    public async Task<MarketSnapshot> SaveAsync(MarketSnapshot snapshot, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        await _collection.InsertOneAsync(snapshot, cancellationToken: cancellationToken);
        return snapshot;
    }

    public async Task<IReadOnlyList<MarketSnapshot>> GetLatestAsync(CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        return await _collection.Aggregate()
            .SortByDescending(x => x.UpdatedAtUtc)
            .Group(x => x.Market, group => group.First())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MarketSnapshot>> GetHistoryAsync(string market, int limit, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        return await _collection.Find(x => x.Market == market)
            .SortByDescending(x => x.UpdatedAtUtc)
            .Limit(Math.Clamp(limit, 1, 1440))
            .ToListAsync(cancellationToken);
    }

    private async Task EnsureIndexesAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.Exchange(ref _indexInitialized, 1) == 1)
        {
            return;
        }

        var marketTime = new CreateIndexModel<MarketSnapshot>(Builders<MarketSnapshot>.IndexKeys.Ascending(x => x.Market).Descending(x => x.UpdatedAtUtc));
        await _collection.Indexes.CreateOneAsync(marketTime, cancellationToken: cancellationToken);
    }
}
