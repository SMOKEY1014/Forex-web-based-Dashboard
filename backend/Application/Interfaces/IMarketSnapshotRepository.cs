using Domain.Entities;

namespace Application.Interfaces;

public interface IMarketSnapshotRepository
{
    Task<MarketSnapshot> SaveAsync(MarketSnapshot snapshot, CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketSnapshot>> GetLatestAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketSnapshot>> GetHistoryAsync(string market, int limit, CancellationToken cancellationToken);
}
