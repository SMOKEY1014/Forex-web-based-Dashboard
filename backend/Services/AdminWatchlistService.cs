using Application.Interfaces;
using Domain.Entities;
using System.Text.Json;

namespace Services;

public sealed class AdminWatchlistService(
    IWatchlistRepository watchlistRepository,
    IWatchlistVersionRepository watchlistVersionRepository)
{
    private static readonly JsonSerializerOptions ExportOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public Task<IReadOnlyList<Watchlist>> GetAllAsync(CancellationToken cancellationToken) =>
        watchlistRepository.GetAllAsync(cancellationToken);

    public Task<Watchlist?> GetByMarketAsync(string market, CancellationToken cancellationToken) =>
        watchlistRepository.GetByMarketAsync(NormalizeMarket(market), cancellationToken);

    public async Task<Watchlist> UpsertAsync(Watchlist watchlist, string operation, CancellationToken cancellationToken)
    {
        var market = NormalizeMarket(watchlist.Market);
        var existing = await watchlistRepository.GetByMarketAsync(market, cancellationToken);
        var nextVersion = await watchlistVersionRepository.GetLatestVersionAsync(market, cancellationToken) + 1;
        var now = DateTimeOffset.UtcNow;

        var normalized = new Watchlist
        {
            Id = existing?.Id ?? watchlist.Id,
            Name = string.IsNullOrWhiteSpace(watchlist.Name) ? market : watchlist.Name,
            Market = market,
            Companies = [.. watchlist.Companies],
            MaximumCompanies = watchlist.MaximumCompanies <= 0 ? 10 : watchlist.MaximumCompanies,
            Notes = watchlist.Notes,
            AutoUpdateEnabled = watchlist.AutoUpdateEnabled,
            AutoWeightRefreshEnabled = watchlist.AutoWeightRefreshEnabled,
            RefreshIntervalSeconds = watchlist.RefreshIntervalSeconds <= 0 ? 30 : watchlist.RefreshIntervalSeconds,
            LastUpdatedUtc = now,
            CreatedUtc = existing?.CreatedUtc ?? now,
            ModifiedUtc = now,
            Version = nextVersion
        };

        var saved = await watchlistRepository.UpsertAsync(normalized, cancellationToken);
        await watchlistVersionRepository.SaveVersionAsync(new WatchlistVersion
        {
            Market = market,
            Version = nextVersion,
            Operation = operation,
            Snapshot = saved,
            ChangedAtUtc = now
        }, cancellationToken);

        return saved;
    }

    public async Task<bool> DeleteAsync(string market, CancellationToken cancellationToken)
    {
        var normalizedMarket = NormalizeMarket(market);
        var existing = await watchlistRepository.GetByMarketAsync(normalizedMarket, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        var deleted = await watchlistRepository.DeleteAsync(normalizedMarket, cancellationToken);
        if (!deleted)
        {
            return false;
        }

        var version = await watchlistVersionRepository.GetLatestVersionAsync(normalizedMarket, cancellationToken) + 1;
        await watchlistVersionRepository.SaveVersionAsync(new WatchlistVersion
        {
            Market = normalizedMarket,
            Version = version,
            Operation = "delete",
            Snapshot = null,
            ChangedAtUtc = DateTimeOffset.UtcNow
        }, cancellationToken);

        return true;
    }

    public Task<IReadOnlyList<WatchlistVersion>> GetVersionsAsync(string market, int limit, CancellationToken cancellationToken) =>
        watchlistVersionRepository.GetVersionsAsync(NormalizeMarket(market), limit, cancellationToken);

    public async Task<Watchlist?> RollbackAsync(string market, int version, CancellationToken cancellationToken)
    {
        var normalizedMarket = NormalizeMarket(market);
        var existingVersion = await watchlistVersionRepository.GetByVersionAsync(normalizedMarket, version, cancellationToken);
        if (existingVersion?.Snapshot is null)
        {
            return null;
        }

        return await UpsertAsync(existingVersion.Snapshot, "rollback", cancellationToken);
    }

    public async Task<string> ExportAsync(CancellationToken cancellationToken)
    {
        var watchlists = await watchlistRepository.GetAllAsync(cancellationToken);
        return JsonSerializer.Serialize(watchlists.OrderBy(w => w.Market), ExportOptions);
    }

    public async Task<IReadOnlyList<Watchlist>> ImportAsync(IReadOnlyList<Watchlist> watchlists, CancellationToken cancellationToken)
    {
        var imported = new List<Watchlist>();
        foreach (var watchlist in watchlists)
        {
            imported.Add(await UpsertAsync(watchlist, "import", cancellationToken));
        }

        return imported;
    }

    private static string NormalizeMarket(string market) => market.Trim().ToUpperInvariant();
}
