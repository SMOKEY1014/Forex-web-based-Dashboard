using Application.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Services;
using SignalR;

namespace BackgroundServices;

public sealed class MarketRefreshWorker(
    ILogger<MarketRefreshWorker> logger,
    IProviderSelector providerSelector,
    IMarketDataProvider marketDataProvider,
    IMarketSnapshotRepository snapshotRepository,
    WatchlistService watchlistService,
    MarketIntelligenceService marketIntelligenceService,
    IHubContext<MarketHub> marketHub,
    IOptionsMonitor<ProviderOptions> providerOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Market refresh worker started with provider {Provider}", providerSelector.GetPrimaryProvider());

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var options = providerOptions.CurrentValue;
                var watchlists = await watchlistService.GetOrSeedWatchlistsAsync(stoppingToken);
                foreach (var watchlist in watchlists)
                {
                    var companies = await marketDataProvider.GetCompaniesAsync(watchlist.Market, stoppingToken);
                    var snapshot = marketIntelligenceService.BuildSnapshot(watchlist.Market, companies, options.UseExtendedHours);
                    await snapshotRepository.SaveAsync(snapshot, stoppingToken);
                    await marketHub.Clients.All.SendAsync("market.snapshot", snapshot, stoppingToken);
                }

                var delay = TimeSpan.FromSeconds(Math.Clamp(options.RefreshIntervalSeconds, 10, 300));
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while refreshing market snapshots");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
